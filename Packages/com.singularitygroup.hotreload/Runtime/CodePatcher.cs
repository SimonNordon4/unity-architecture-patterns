#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using JetBrains.Annotations;
using SingularityGroup.HotReload.Burst;
using SingularityGroup.HotReload.HarmonyLib;
using SingularityGroup.HotReload.JsonConverters;
using SingularityGroup.HotReload.Newtonsoft.Json;
using SingularityGroup.HotReload.RuntimeDependencies;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.Editor")]

namespace SingularityGroup.HotReload {
    class RegisterPatchesResult {
        // note: doesn't include removals and method definition changes (e.g. renames)
        public readonly List<MethodPatch> patchedMethods = new List<MethodPatch>();
        public readonly List<SMethod> patchedSMethods = new List<SMethod>();
        public readonly List<Tuple<SMethod, string>> patchFailures = new List<Tuple<SMethod, string>>();
    }
    
    class CodePatcher {
        public static readonly CodePatcher I = new CodePatcher();
        /// <summary>Tag for use in Debug.Log.</summary>
        public const string TAG = "HotReload";
        
        internal int PatchesApplied { get; private set; }
        string PersistencePath {get;}
        
        List<MethodPatchResponse> pendingPatches;
        readonly List<MethodPatchResponse> patchHistory;
        readonly HashSet<string> seenResponses = new HashSet<string>();
        string[] assemblySearchPaths;
        SymbolResolver symbolResolver;
        readonly string tmpDir;
        
        CodePatcher() {
            pendingPatches = new List<MethodPatchResponse>();
            patchHistory = new List<MethodPatchResponse>(); 
            if(UnityHelper.IsEditor) {
                tmpDir = PackageConst.LibraryCachePath;
            } else {
                tmpDir = UnityHelper.TemporaryCachePath;
            }
            if(!UnityHelper.IsEditor) {
                PersistencePath = Path.Combine(UnityHelper.PersistentDataPath, "HotReload", "patches.json");
                try {
                    LoadPatches(PersistencePath);
                } catch(Exception ex) {
                    Log.Error("Encountered exception when loading patches from disk:\n{0}", ex);
                }
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeUnityEvents() {
            UnityEventHelper.Initialize();
        }

        
        void LoadPatches(string filePath) {
            PlayerLog("Loading patches from file {0}", filePath);
            var file = new FileInfo(filePath);
            if(file.Exists) {
                var bytes = File.ReadAllText(filePath);
                var patches = JsonConvert.DeserializeObject<List<MethodPatchResponse>>(bytes);
                PlayerLog("Loaded {0} patches from disk", patches.Count.ToString());
                foreach (var patch in patches) {
                    RegisterPatches(patch, persist: false);
                }
            }  
        }

        
        internal IReadOnlyList<MethodPatchResponse> PendingPatches => pendingPatches;
        internal SymbolResolver SymbolResolver => symbolResolver;
        
        
        internal string[] GetAssemblySearchPaths() {
            EnsureSymbolResolver();
            return assemblySearchPaths;
        }
       
        internal RegisterPatchesResult RegisterPatches(MethodPatchResponse patches, bool persist) {
            PlayerLog("Register patches.\nWarnings: {0} \nMethods:\n{1}", string.Join("\n", patches.failures), string.Join("\n", patches.patches.SelectMany(p => p.modifiedMethods).Select(m => m.displayName)));
            pendingPatches.Add(patches);
            return ApplyPatches(persist);
        }
        
        RegisterPatchesResult ApplyPatches(bool persist) {
            PlayerLog("ApplyPatches. {0} patches pending.", pendingPatches.Count);
            EnsureSymbolResolver();

            var result = new RegisterPatchesResult();
            
            try {
                int count = 0;
                foreach(var response in pendingPatches) {
                    if (seenResponses.Contains(response.id)) {
                        continue;
                    }
                    HandleMethodPatchResponse(response, result);
                    patchHistory.Add(response);

                    seenResponses.Add(response.id);
                    count += response.patches.Length;
                }
                if (count > 0) {
                    Dispatch.OnHotReload(result.patchedMethods).Forget();
                }
            } catch(Exception ex) {
                Log.Warning("Exception occured when handling method patch. Exception:\n{0}", ex);
            } finally {
                pendingPatches.Clear();
            }
            
            if(PersistencePath != null && persist) {
                SaveAppliedPatches(PersistencePath).Forget();
            }

            PatchesApplied++;
            return result;
        }
        
        internal void ClearPatchedMethods() {
            PatchesApplied = 0;
        }

        static bool didLog;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void WarnOnSceneLoad() {
            SceneManager.sceneLoaded += (_, __) => {
                if (didLog || !UnityEventHelper.UnityMethodsAdded()) {
                    return;
                }
                Log.Warning("A new Scene was loaded while new unity event methods were added at runtime. MonoBehaviours in the Scene will not trigger these new events.");
                didLog = true;
            };
        }

        void HandleMethodPatchResponse(MethodPatchResponse response, RegisterPatchesResult result) {
            EnsureSymbolResolver();

            foreach(var patch in response.patches) {
                try {
                    var asm = Assembly.Load(patch.patchAssembly, patch.patchPdb);

                    var module = asm.GetLoadedModules()[0];
                    foreach(var sMethod in patch.newMethods) {
                        var newMethod = module.ResolveMethod(sMethod.metadataToken);
                        try {
                            UnityEventHelper.EnsureUnityEventMethod(newMethod);
                        } catch(Exception ex) {
                            Log.Warning("Encountered exception in EnsureUnityEventMethod: {0} {1}", ex.GetType().Name, ex.Message);
                        }
                        MethodUtils.DisableVisibilityChecks(newMethod);
                        if (!patch.patchMethods.Any(m => m.metadataToken == sMethod.metadataToken)) {
                            result.patchedMethods.Add(new MethodPatch(null, null, newMethod));
                            result.patchedSMethods.Add(sMethod);
                            previousPatchMethods[newMethod] = newMethod;
                            newMethods.Add(newMethod);
                        }
                    }
                    
                    symbolResolver.AddAssembly(asm);
                    for (int i = 0; i < patch.modifiedMethods.Length; i++) {
                        var sOriginalMethod = patch.modifiedMethods[i];
                        var sPatchMethod = patch.patchMethods[i];
                        var err = PatchMethod(module: module, sOriginalMethod: sOriginalMethod, sPatchMethod: sPatchMethod, containsBurstJobs: patch.unityJobs.Length > 0, patchesResult: result);
                        if (!string.IsNullOrEmpty(err)) {
                            result.patchFailures.Add(Tuple.Create(sOriginalMethod, err));
                        }
                    }
                    JobHotReloadUtility.HotReloadBurstCompiledJobs(patch, module);
                } catch(Exception ex) {
                    Log.Warning("Failed to apply patch with id: {0}\n{1}", patch.patchId, ex);
                }
            }
        }

        Dictionary<MethodBase, MethodBase> previousPatchMethods = new Dictionary<MethodBase, MethodBase>();
        List<MethodBase> newMethods = new List<MethodBase>();

        string PatchMethod(Module module, SMethod sOriginalMethod, SMethod sPatchMethod, bool containsBurstJobs, RegisterPatchesResult patchesResult) {
            try {
                var patchMethod = module.ResolveMethod(sPatchMethod.metadataToken);
                var start = DateTime.UtcNow;
                var state = TryResolveMethod(sOriginalMethod, patchMethod);

                if (DateTime.UtcNow - start > TimeSpan.FromMilliseconds(500)) {
                    Log.Info("Hot Reload apply took {0}", (DateTime.UtcNow - start).TotalMilliseconds);
                }

                if(state.match == null) {
                    var error = 
                        "Method mismatch: {0}, patch: {1}. This can have multiple reasons:\n"
                        + "1. You are running the Editor multiple times for the same project using symlinks, and are making changes from the symlink project\n"
                        + "2. A bug in Hot Reload. Please send us a reproduce (code before/after), and we'll get it fixed for you\n"
                        ;
                    Log.Warning(error, sOriginalMethod.simpleName, patchMethod.Name);

                    return string.Format(error, sOriginalMethod.simpleName, patchMethod.Name);
                }

                PlayerLog("Detour method {0:X8} {1}, offset: {2}", sOriginalMethod.metadataToken, patchMethod.Name, state.offset);
                DetourResult result;
                DetourApi.DetourMethod(state.match, patchMethod, out result);
                if (result.success) {
                    // previous method is either original method or the last patch method
                    MethodBase previousMethod;
                    if (!previousPatchMethods.TryGetValue(state.match, out previousMethod)) {
                        previousMethod = state.match;
                    }
                    MethodBase originalMethod = state.match;
                    if (newMethods.Contains(state.match)) {
                        // for function added at runtime the original method should be null
                        originalMethod = null;
                    }
                    patchesResult.patchedMethods.Add(new MethodPatch(originalMethod, previousMethod, patchMethod));
                    patchesResult.patchedSMethods.Add(sOriginalMethod);
                    previousPatchMethods[state.match] = patchMethod;
                    try {
                        Dispatch.OnHotReloadLocal(state.match, patchMethod);
                    } catch {
                        // best effort
                    }
                    return null;
                } else {
                    if(result.exception is InvalidProgramException && containsBurstJobs) {
                        //ignore. The method is likely burst compiled and can't be patched
                        return null;
                    } else {
                        return HandleMethodPatchFailure(sOriginalMethod, result.exception);
                    }
                }
            } catch(Exception ex) {
                return HandleMethodPatchFailure(sOriginalMethod, ex);
            }
        }
        
        struct ResolveMethodState {
            public readonly SMethod originalMethod;
            public readonly int offset;
            public readonly bool tryLowerTokens;
            public readonly bool tryHigherTokens;
            public readonly MethodBase match;
            public ResolveMethodState(SMethod originalMethod, int offset, bool tryLowerTokens, bool tryHigherTokens, MethodBase match) {
                this.originalMethod = originalMethod;
                this.offset = offset;
                this.tryLowerTokens = tryLowerTokens;
                this.tryHigherTokens = tryHigherTokens;
                this.match = match;
            }

            public ResolveMethodState With(bool? tryLowerTokens = null, bool? tryHigherTokens = null, MethodBase match = null, int? offset = null) {
                return new ResolveMethodState(
                    originalMethod, 
                    offset ?? this.offset, 
                    tryLowerTokens ?? this.tryLowerTokens,
                    tryHigherTokens ?? this.tryHigherTokens,
                    match ?? this.match);
            }
        }
        
        struct ResolveMethodResult {
            public readonly MethodBase resolvedMethod;
            public readonly bool tokenOutOfRange;
            public ResolveMethodResult(MethodBase resolvedMethod, bool tokenOutOfRange) {
                this.resolvedMethod = resolvedMethod;
                this.tokenOutOfRange = tokenOutOfRange;
            }
        }
        
        ResolveMethodState TryResolveMethod(SMethod originalMethod, MethodBase patchMethod) {
            var state = new ResolveMethodState(originalMethod, offset: 0, tryLowerTokens: true, tryHigherTokens: true, match: null);
            var result = TryResolveMethodCore(state.originalMethod, patchMethod, 0);
            if(result.resolvedMethod != null) {
                return state.With(match: result.resolvedMethod);
            }
            state = state.With(offset: 1);
            const int tries = 100000;
            while(state.offset <= tries && (state.tryHigherTokens || state.tryLowerTokens)) {
                if(state.tryHigherTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryHigherTokens: false);
                    }
                }
                if(state.tryLowerTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, -state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryLowerTokens: false);
                    }
                }
                state = state.With(offset: state.offset + 1);
            }
            return state;
        }
        
        
        ResolveMethodResult TryResolveMethodCore(SMethod methodToResolve, MethodBase patchMethod, int offset) {
            bool tokenOutOfRange = false;
            MethodBase resolvedMethod = null;
            try {
                resolvedMethod = TryGetMethodBaseWithRelativeToken(methodToResolve, offset);
                if(!MethodCompatiblity.AreMethodsCompatible(resolvedMethod, patchMethod)) {
                    resolvedMethod = null;
                }
            } catch (SymbolResolvingFailedException ex) when(ex.InnerException is ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            } catch (ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            }
            return new ResolveMethodResult(resolvedMethod, tokenOutOfRange);
        }
        
        MethodBase TryGetMethodBaseWithRelativeToken(SMethod sOriginalMethod, int offset) {
            return symbolResolver.Resolve(new SMethod(sOriginalMethod.assemblyName, 
                sOriginalMethod.displayName, 
                sOriginalMethod.metadataToken + offset,
                sOriginalMethod.genericTypeArguments, 
                sOriginalMethod.genericTypeArguments,
                sOriginalMethod.simpleName));
        }
    
        string HandleMethodPatchFailure(SMethod method, Exception exception) {
            var err = $"Failed to apply patch for method {method.displayName} in assembly {method.assemblyName}\n{exception}";
            Log.Warning(err);
            return err;
        }

        void EnsureSymbolResolver() {
            if (symbolResolver == null) {
                var searchPaths = new HashSet<string>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assembliesByName = new Dictionary<string, List<Assembly>>();
                for (var i = 0; i < assemblies.Length; i++) {
                    var name = assemblies[i].GetNameSafe();
                    List<Assembly> list;
                    if (!assembliesByName.TryGetValue(name, out list)) {
                        assembliesByName.Add(name, list = new List<Assembly>());
                    }
                    list.Add(assemblies[i]);
                    
                    if(assemblies[i].IsDynamic) continue;

                    var location = assemblies[i].Location;
                    if(File.Exists(location)) {
                        searchPaths.Add(Path.GetDirectoryName(Path.GetFullPath(location)));
                    }
                }
                symbolResolver = new SymbolResolver(assembliesByName);
                assemblySearchPaths = searchPaths.ToArray();
            }
        }
        
        
        //Allow one save operation at a time.
        readonly SemaphoreSlim gate = new SemaphoreSlim(1);
        public async Task SaveAppliedPatches(string filePath) {
            await gate.WaitAsync();
            try {
                await SaveAppliedPatchesNoLock(filePath);
            } finally {
                gate.Release();
            }
        }
        
        async Task SaveAppliedPatchesNoLock(string filePath) {
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }
            filePath = Path.GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            if(string.IsNullOrEmpty(dir)) {
                throw new ArgumentException("Invalid path: " + filePath, nameof(filePath));
            }
            Directory.CreateDirectory(dir);
            var history = patchHistory.ToList();
            
            PlayerLog("Saving {0} applied patches to {1}", history.Count, filePath);

            await Task.Run(() => {
                using (FileStream fs = File.Create(filePath))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter writer = new JsonTextWriter(sw)) {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                        Converters = new List<JsonConverter> { new MethodPatchResponsesConverter() }
                    });
                    serializer.Serialize(writer, history);
                }
            });
        }
        
        public void InitPatchesBlocked(string filePath) {
            seenResponses.Clear();
            var file = new FileInfo(filePath);
            if (file.Exists) {
                using(var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                using (StreamReader sr = new StreamReader(fs))
                using (JsonReader reader = new JsonTextReader(sr)) {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                        Converters = new List<JsonConverter> { new MethodPatchResponsesConverter() }
                    });
                    pendingPatches = serializer.Deserialize<List<MethodPatchResponse>>(reader);
                }
                ApplyPatches(persist: false);
            }
        }
        
        
        [StringFormatMethod("format")]
        static void PlayerLog(string format, params object[] args) {
#if !UNITY_EDITOR
            HotReload.Log.Info(format, args);
#endif //!UNITY_EDITOR
        }
        
        class SimpleMethodComparer : IEqualityComparer<SMethod> {
            public static readonly SimpleMethodComparer I = new SimpleMethodComparer();
            SimpleMethodComparer() { }
            public bool Equals(SMethod x, SMethod y) => x.metadataToken == y.metadataToken;
            public int GetHashCode(SMethod x) {
                return x.metadataToken;
            }
        }
    }
}
#endif
