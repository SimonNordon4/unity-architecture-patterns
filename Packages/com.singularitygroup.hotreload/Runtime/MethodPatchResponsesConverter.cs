#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;

namespace SingularityGroup.HotReload.JsonConverters {
    internal class MethodPatchResponsesConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(List<MethodPatchResponse>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var list = new List<MethodPatchResponse>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadMethodPatchResponse(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SMethod list
                }
            }

            return list;
        }
        
        private MethodPatchResponse ReadMethodPatchResponse(JsonReader reader) {
            string id = null;
            CodePatch[] patches = null;
            string[] failures = null;
            SMethod[] removedMethod = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(MethodPatchResponse.id):
                        id = reader.ReadAsString();
                        break;

                    case nameof(MethodPatchResponse.patches):
                        patches = ReadPatches(reader);
                        break;

                    case nameof(MethodPatchResponse.failures):
                        failures = ReadStringArray(reader);
                        break;

                    case nameof(MethodPatchResponse.removedMethod):
                        removedMethod = ReadSMethodArray(reader);
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }
            
            return new MethodPatchResponse(
                id ?? string.Empty,
                patches ?? Array.Empty<CodePatch>(), 
                failures ?? Array.Empty<string>(), 
                removedMethod ?? Array.Empty<SMethod>(),
                // Note: doesn't have to be persisted here 
                Array.Empty<PartiallySupportedChange>()
            );
        }

        private CodePatch[] ReadPatches(JsonReader reader) {
            var patches = new List<CodePatch>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndArray) {
                    break;
                }
                if (reader.TokenType != JsonToken.StartObject) {
                    continue;
                }
                string patchId = null;
                string assemblyName = null;
                byte[] patchAssembly = null;
                byte[] patchPdb = null;
                SMethod[] modifiedMethods = null;
                SMethod[] patchMethods = null;
                SMethod[] newMethods = null;
                SUnityJob[] unityJobs = null;

                while (reader.Read()) {
                    if (reader.TokenType == JsonToken.EndObject) {
                        break;
                    }
                    if (reader.TokenType != JsonToken.PropertyName) {
                        continue;
                    }
                    var propertyName = (string)reader.Value;

                    switch (propertyName) {
                        case nameof(CodePatch.patchId):
                            patchId = reader.ReadAsString();
                            break;

                        case nameof(CodePatch.assemblyName):
                            assemblyName = reader.ReadAsString();
                            break;
                        
                        case nameof(CodePatch.patchAssembly):
                            patchAssembly = Convert.FromBase64String(reader.ReadAsString());
                            break;
                        
                        case nameof(CodePatch.patchPdb):
                            patchPdb = Convert.FromBase64String(reader.ReadAsString());
                            break;
                        
                        case nameof(CodePatch.modifiedMethods):
                            modifiedMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.patchMethods):
                            patchMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.newMethods):
                            newMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.unityJobs):
                            unityJobs = ReadSUnityJobArray(reader);
                            break;

                        default:
                            reader.Skip(); // Skip unknown properties
                            break;
                    }
                }

                patches.Add(new CodePatch(
                    patchId: patchId ?? string.Empty,
                    assemblyName: assemblyName ?? string.Empty,
                    patchAssembly: patchAssembly ?? Array.Empty<byte>(),
                    patchPdb: patchPdb ?? Array.Empty<byte>(),
                    modifiedMethods: modifiedMethods ?? Array.Empty<SMethod>(),
                    patchMethods: patchMethods ?? Array.Empty<SMethod>(),
                    newMethods: newMethods ?? Array.Empty<SMethod>(),
                    unityJobs: unityJobs ?? Array.Empty<SUnityJob>()
                ));
            }

            return patches.ToArray();
        }

        private string[] ReadStringArray(JsonReader reader) {
            var list = new List<string>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.String) {
                    list.Add((string)reader.Value);
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the string list
                }
            }

            return list.ToArray();
        }

        private SMethod[] ReadSMethodArray(JsonReader reader) {
            var list = new List<SMethod>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadSMethod(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SMethod list
                }
            }

            return list.ToArray();
        }

        private SType[] ReadSTypeArray(JsonReader reader) {
            var list = new List<SType>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadSType(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SType list
                }
            }

            return list.ToArray();
        }
        
        private SUnityJob[] ReadSUnityJobArray(JsonReader reader) {
            var array = new List<SUnityJob>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    array.Add(ReadSUnityJob(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SUnityJob array
                }
            }

            return array.ToArray();
        }

        private SMethod ReadSMethod(JsonReader reader) {
            string assemblyName = null;
            string displayName = null;
            int metadataToken = default(int);
            SType[] genericTypeArguments = null;
            SType[] genericArguments = null;
            string simpleName = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SMethod.assemblyName):
                        assemblyName = reader.ReadAsString();
                        break;

                    case nameof(SMethod.displayName):
                        displayName = reader.ReadAsString();
                        break;
                    
                    case nameof(SMethod.metadataToken):
                        metadataToken = reader.ReadAsInt32() ?? default(int);
                        break;
                    
                    case nameof(SMethod.genericTypeArguments):
                        genericTypeArguments = ReadSTypeArray(reader);
                        break;
                    
                    case nameof(SMethod.genericArguments):
                        genericArguments = ReadSTypeArray(reader);
                        break;
                    
                    case nameof(SMethod.simpleName):
                        simpleName = reader.ReadAsString();
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SMethod(
                assemblyName ?? string.Empty,
                displayName ?? string.Empty,
                metadataToken, 
                genericTypeArguments ?? Array.Empty<SType>(),
                genericArguments ?? Array.Empty<SType>(),
                simpleName ?? string.Empty
            );
        }

        private SType ReadSType(JsonReader reader) {
            string assemblyName = null;
            string typeName = null;
            SType[] genericArguments = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SType.assemblyName):
                        assemblyName = reader.ReadAsString();
                        break;

                    case nameof(SType.typeName):
                        typeName = reader.ReadAsString();
                        break;

                    case nameof(SType.genericArguments):
                        genericArguments = ReadSTypeArray(reader);
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SType(
                assemblyName ?? string.Empty,
                typeName ?? string.Empty,
                genericArguments ?? Array.Empty<SType>()
            );
        }

        private SUnityJob ReadSUnityJob(JsonReader reader) {
            int metadataToken = default(int);
            UnityJobKind jobKind = default(UnityJobKind);

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SUnityJob.metadataToken):
                        metadataToken = reader.ReadAsInt32() ?? 0;
                        break;

                    case nameof(SUnityJob.jobKind):
                        var jobKindStr = reader.ReadAsString();
                        Enum.TryParse(jobKindStr, out jobKind);
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SUnityJob(metadataToken, jobKind);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var responses = (List<MethodPatchResponse>)value;
            if (responses == null) {
                writer.WriteNull();
                return;
            }
            
            writer.WriteStartArray();
            foreach (var response in responses) {
                writer.WriteStartObject();
                
                writer.WritePropertyName(nameof(response.id));
                writer.WriteValue(response.id);

                if (response.patches != null) {
                    writer.WritePropertyName(nameof(response.patches));
                    writer.WriteStartArray();
                    foreach (var responsePatch in response.patches) {
                        writer.WriteStartObject();
                        
                        writer.WritePropertyName(nameof(responsePatch.patchId));
                        writer.WriteValue(responsePatch.patchId);
                        writer.WritePropertyName(nameof(responsePatch.assemblyName));
                        writer.WriteValue(responsePatch.assemblyName);
                        writer.WritePropertyName(nameof(responsePatch.patchAssembly));
                        writer.WriteValue(Convert.ToBase64String(responsePatch.patchAssembly));
                        writer.WritePropertyName(nameof(responsePatch.patchPdb));
                        writer.WriteValue(Convert.ToBase64String(responsePatch.patchPdb));

                        if (responsePatch.modifiedMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.modifiedMethods));
                            writer.WriteStartArray();
                            foreach (var modifiedMethod in responsePatch.modifiedMethods) {
                                WriteSMethod(writer, modifiedMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.patchMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.patchMethods));
                            writer.WriteStartArray();
                            foreach (var patchMethod in responsePatch.patchMethods) {
                                WriteSMethod(writer, patchMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.newMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.newMethods));
                            writer.WriteStartArray();
                            foreach (var newMethod in responsePatch.newMethods) {
                                WriteSMethod(writer, newMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.unityJobs != null) {
                            writer.WritePropertyName(nameof(responsePatch.unityJobs));
                            writer.WriteStartArray();
                            foreach (var unityJob in responsePatch.unityJobs) {
                                writer.WriteStartObject();

                                writer.WritePropertyName(nameof(unityJob.metadataToken));
                                writer.WriteValue(unityJob.metadataToken);
                                writer.WritePropertyName(nameof(unityJob.jobKind));
                                writer.WriteValue(unityJob.jobKind.ToString());

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                        }

                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }

                if (response.failures != null) {
                    writer.WritePropertyName(nameof(response.failures));
                    writer.WriteStartArray();
                    foreach (var failure in response.failures) {
                        writer.WriteValue(failure);
                    }
                    writer.WriteEndArray();
                }

                if (response.removedMethod != null) {
                    writer.WritePropertyName(nameof(response.removedMethod));
                    writer.WriteStartArray();
                    foreach (var removedMethod in response.removedMethod) {
                        WriteSMethod(writer, removedMethod);
                    }
                    writer.WriteEndArray();
                }
                
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        
        void WriteSMethod(JsonWriter writer, SMethod method) {
            writer.WriteStartObject();
            
            writer.WritePropertyName(nameof(method.assemblyName));
            writer.WriteValue(method.assemblyName);
            writer.WritePropertyName(nameof(method.displayName));
            writer.WriteValue(method.displayName);
            writer.WritePropertyName(nameof(method.metadataToken));
            writer.WriteValue(method.metadataToken);

            if (method.genericTypeArguments != null) {
                writer.WritePropertyName(nameof(method.genericTypeArguments));
                writer.WriteStartArray();
                foreach (var genericTypeArgument in method.genericTypeArguments) {
                    WriteSType(writer, genericTypeArgument);
                }
                writer.WriteEndArray();
            }

            if (method.genericArguments != null) {
                writer.WritePropertyName(nameof(method.genericArguments));
                writer.WriteStartArray();
                foreach (var genericArgument in method.genericArguments) {
                    WriteSType(writer, genericArgument);
                }
                writer.WriteEndArray();
            }
            
            writer.WritePropertyName(nameof(method.simpleName));
            writer.WriteValue(method.simpleName);
            
            writer.WriteEndObject();
        }

        void WriteSType(JsonWriter writer, SType type) {
            writer.WriteStartObject();
            
            writer.WritePropertyName(nameof(type.assemblyName));
            writer.WriteValue(type.assemblyName);
            writer.WritePropertyName(nameof(type.typeName));
            writer.WriteValue(type.typeName);

            // always writing generic arguments will cause recursion issues
            if (type.genericArguments?.Length > 0) {
                writer.WritePropertyName(nameof(type.genericArguments));
                writer.WriteStartArray();
                foreach (var genericArgument in type.genericArguments) {
                    WriteSType(writer, genericArgument);
                }
                writer.WriteEndArray();
            }
            
            writer.WriteEndObject();
        }
    }
}
#endif
