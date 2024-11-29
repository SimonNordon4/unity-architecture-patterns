using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {

    public enum HotReloadSuggestionKind {
        UnsupportedChanges,
        UnsupportedPackages,
        [Obsolete] SymbolicLinks,
        AutoRecompiledWhenPlaymodeStateChanges,
        UnityBestDevelopmentToolAward2023,
#if UNITY_2022_1_OR_NEWER
        AutoRecompiledWhenPlaymodeStateChanges2022,
#endif
        MultidimensionalArrays,
        EditorsWithoutHRRunning,
    }
    
	internal static class HotReloadSuggestionsHelper {
        internal static void SetSuggestionsShown(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}")) {
                return;
            }
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", true);
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}", true);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static bool CheckSuggestionActive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            return EditorPrefs.GetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}");
        }
        
        // used for cases where suggestion might need to be shown more than once
        internal static void SetSuggestionActive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}")) {
                return;
            }
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", true);
            
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static void SetSuggestionInactive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", false);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry)) {
                HotReloadTimelineHelper.Suggestions.Remove(entry);
            }
        }
        
        internal static void InitSuggestions() {
            foreach (HotReloadSuggestionKind value in Enum.GetValues(typeof(HotReloadSuggestionKind))) {
                if (!CheckSuggestionActive(value)) {
                    continue;
                }
                AlertEntry entry;
                if (suggestionMap.TryGetValue(value, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                    HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                }
            }
        }
        
        internal static HotReloadSuggestionKind? FindSuggestionKind(AlertEntry targetEntry) {
            foreach (KeyValuePair<HotReloadSuggestionKind, AlertEntry> pair in suggestionMap) {
                if (pair.Value.Equals(targetEntry)) {
                    return pair.Key;
                }
            }
            return null;
        }
        
        internal static readonly OpenURLButton recompileTroubleshootingButton = new OpenURLButton("Docs", Constants.RecompileTroubleshootingURL);
        internal static readonly OpenURLButton featuresDocumentationButton = new OpenURLButton("Docs", Constants.FeaturesDocumentationURL);
        internal static readonly OpenURLButton multipleEditorsDocumentationButton = new OpenURLButton("Docs", Constants.MultipleEditorsURL);
        public static Dictionary<HotReloadSuggestionKind, AlertEntry> suggestionMap = new Dictionary<HotReloadSuggestionKind, AlertEntry> {
            { HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023, new AlertEntry(
                AlertType.Suggestion, 
                "Vote for the \"Best Development Tool\" Award!", 
                "Hot Reload was nominated for the \"Best Development Tool\" Award. Please consider voting. Thank you!",
                actionData: () => {
                    GUILayout.Space(6f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Vote ")) {
                            Application.OpenURL(Constants.VoteForAwardURL);
                            SetSuggestionInactive(HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023);
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Which changes does Hot Reload support?", 
                "Hot Reload supports most code changes, but there are some limitations. Generally, changes to the method definition and body are allowed. Non-method changes (like adding/editing classes and fields) are not supported. See the documentation for the list of current features and our current roadmap",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        featuresDocumentationButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedPackages, new AlertEntry(
                AlertType.Suggestion, 
                "Unsupported package detected",
                "The following packages are only partially supported: ECS, Mirror, Fishnet, and Photon. Hot Reload will work in the project, but changes specific to those packages might not work. Contact us if these packages are a big part of your project",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Unity recompiles on enter/exit play mode?",
                "If you have an issue with the Unity Editor recompiling when the Play Mode state changes, please consult the documentation, and don’t hesitate to reach out to us if you need assistance",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        recompileTroubleshootingButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.discordButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
#if UNITY_2022_1_OR_NEWER
            { HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022, new AlertEntry(
                AlertType.Suggestion, 
                "Unsupported setting detected",
                "The 'Sprite Packer Mode' setting can cause unintended recompilations if set to 'Sprite Atlas V1 - Always Enabled'",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Use \"Sprite Atlas V2\" ")) {
                            EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;
                        }
                        if (GUILayout.Button(" Open Settings ")) {
                            SettingsService.OpenProjectSettings("Project/Editor");
                        }
                        if (GUILayout.Button(" Ignore suggestion ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
                        }
                        
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                hasExitButton: false
            )},
#endif
            { HotReloadSuggestionKind.MultidimensionalArrays, new AlertEntry(
                AlertType.Suggestion, 
                "Use jagged instead of multidimensional arrays",
                "Hot Reload doesn't support multidimensional ([,]) arrays. Jagged arrays ([][]) are a better alternative, and Microsoft recommends using them instead",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Learn more ")) {
                            Application.OpenURL("https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1814");
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.EditorsWithoutHRRunning, new AlertEntry(
                AlertType.Suggestion, 
                "Some Unity instances don't have Hot Reload running.",
                "Make sure that either: \n1) Hot Reload is installed and running on all Editor instances, or \n2) Hot Reload is stopped in all Editor instances where it is installed.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Stop Hot Reload ")) {
                            EditorCodePatcher.StopCodePatcher().Forget();
                        }
                        GUILayout.Space(5f);
                        
                        multipleEditorsDocumentationButton.OnGUI();
                        GUILayout.Space(5f);
                        
                        if (GUILayout.Button(" Don't show again ")) {
                            HotReloadSuggestionsHelper.SetSuggestionsShown(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                            HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.UnsupportedChange
            )},
        };
        
        static ListRequest listRequest;
        static string[] unsupportedPackages = new[] {
            "com.unity.entities",
            "com.firstgeargames.fishnet",
        };
        static List<string> unsupportedPackagesList;
        static DateTime lastPlaymodeChange;
        
        public static void Init() {
            listRequest = Client.List(offlineMode: false, includeIndirectDependencies: true);

            EditorApplication.playModeStateChanged += state => {
                lastPlaymodeChange = DateTime.UtcNow;
            };
            CompilationPipeline.compilationStarted += obj => {
                if (DateTime.UtcNow - lastPlaymodeChange < TimeSpan.FromSeconds(1) && !HotReloadState.RecompiledUnsupportedChangesOnExitPlaymode) {
                    
#if UNITY_2022_1_OR_NEWER
                    SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
#else
                    SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges);
#endif
                }
                HotReloadState.RecompiledUnsupportedChangesOnExitPlaymode = false;
            };
            InitSuggestions();
        }

        private static DateTime lastCheckedUnityInstances = DateTime.UtcNow;
        public static void Check() {
            if (listRequest.IsCompleted && 
                unsupportedPackagesList == null) 
            {
                unsupportedPackagesList = new List<string>();
                var packages = listRequest.Result;
                foreach (var packageInfo in packages) {
                    if (unsupportedPackages.Contains(packageInfo.name)) {
                        unsupportedPackagesList.Add(packageInfo.name);
                    }
                }
                if (unsupportedPackagesList.Count > 0) {
                    SetSuggestionsShown(HotReloadSuggestionKind.UnsupportedPackages);
                }
            }
            
            CheckEditorsWithoutHR();

#if UNITY_2022_1_OR_NEWER
            if (EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOnAtlas) {
                SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
            } else if (CheckSuggestionActive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022)) { 
                SetSuggestionInactive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
                EditorPrefs.SetBool($"HotReloadWindow.SuggestionsShown.{HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022}", false);
            }
#endif
        }
        
        private static void CheckEditorsWithoutHR() {
            if (!ServerHealthCheck.I.IsServerHealthy) {
                HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                return;
            }
            if (checkingEditorsWihtoutHR || 
                (DateTime.UtcNow - lastCheckedUnityInstances).TotalSeconds < 5)
            {
                return;
            }
            CheckEditorsWithoutHRAsync().Forget();
        }

        static bool checkingEditorsWihtoutHR;
        private static async Task CheckEditorsWithoutHRAsync() {
            try {
                checkingEditorsWihtoutHR = true;
                var showSuggestion = await Task.Run(() => {
                    try {
                        var runningUnities = Process.GetProcessesByName("Unity").Length;
                        var runningPatchers = Process.GetProcessesByName("CodePatcherCLI").Length;
                        return runningPatchers > 0 && runningUnities > runningPatchers;
                    } catch (ArgumentException) {
                        // On some devices GetProcessesByName throws ArgumentException for no good reason.
                        // it happens rarely and the feature is not the most important so proper solution is not required
                        return false;
                    }
                });
                if (!showSuggestion) {
                    HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                    return;
                }
                if (!HotReloadState.ShowedEditorsWithoutHR && ServerHealthCheck.I.IsServerHealthy) {
                    HotReloadSuggestionsHelper.SetSuggestionActive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                    HotReloadState.ShowedEditorsWithoutHR = true;
                }
            } finally {
                checkingEditorsWihtoutHR = false;
                lastCheckedUnityInstances = DateTime.UtcNow;
            }
        }
	}
}
