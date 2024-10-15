#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Reflection;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.RuntimeDependencies;

namespace SingularityGroup.HotReload {
    internal class SymbolResolver {
        readonly Dictionary<string, List<Assembly>> assembliesByName;

        public SymbolResolver(Dictionary<string, List<Assembly>> assembliesByName) {
            this.assembliesByName = assembliesByName;
        }
        
        public void AddAssembly(Assembly asm) {
            var asmName = asm.GetNameSafe();
            List<Assembly> assemblies;
            if(!assembliesByName.TryGetValue(asmName, out assemblies)) {
                assembliesByName.Add(asmName, assemblies = new List<Assembly>());
            }
            assemblies.Add(asm);
        }

        public Type Resolve(SType t) {
            List<Assembly> assemblies;
            if (assembliesByName.TryGetValue(t.assemblyName, out assemblies)) {
              
                Type type;
                foreach (var assembly in assemblies) {
                    if ((type = assembly.GetType(t.typeName)) != null) {
                        if(t.typeName == "System.Array" && t.genericArguments.Length > 0) {
                            var elementType = Resolve(t.genericArguments[0]);
                            return elementType.Assembly.GetType(t.genericArguments[0].typeName + "[]");
                        }
                        if(t.genericArguments.Length > 0) {
                            type = type.MakeGenericType(ResolveTypes(t.genericArguments));
                        }
                        return type;
                    }
                }
            }
            throw new SymbolResolvingFailedException(t);
        }
        
        public IReadOnlyList<Assembly> Resolve(string assembly) {
            List<Assembly> list;
            if(assembliesByName.TryGetValue(assembly, out list)) {
                return list;
            }
            return Array.Empty<Assembly>();
        }
        
        public MethodBase Resolve(SMethod m) {
            var assmeblies = Resolve(m.assemblyName);
            var genericTypeArgs = ResolveTypes(m.genericTypeArguments);
            var genericMethodArgs = ResolveTypes(m.genericArguments);
            MethodBase result = null;
            Exception lastException = null;
            for (var i = 0; i < assmeblies.Count; i++) {
                try {
                    result = assmeblies[i].GetLoadedModules()[0].ResolveMethod(m.metadataToken, genericTypeArgs, genericMethodArgs);
                    break;
                } catch(Exception ex) {
                    lastException = ex;
                }
            }
            if(result == null) {
                throw new SymbolResolvingFailedException(m, lastException);
            }
            return result;
        }

        Type[] ResolveTypes(SType[] sTypes) {
            if(sTypes == null) {
                return null;
            }
            if(sTypes.Length == 0) {
                return Array.Empty<Type>();
            }
            var result = new Type[sTypes.Length];
            for (int i = 0; i < sTypes.Length; i++) {
                result[i] = Resolve(sTypes[i]);
            }
            return result;
        }
    }
}
#endif
