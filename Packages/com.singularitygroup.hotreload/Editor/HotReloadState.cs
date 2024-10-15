using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadState {
        private const string ServerPortKey = "HotReloadWindow.ServerPort";
        private const string LastPatchIdKey = "HotReloadWindow.LastPatchId";
        private const string ShowingRedDotKey = "HotReloadWindow.ShowingRedDot";
        private const string ShowedEditorsWithoutHRKey = "HotReloadWindow.ShowedEditorWithoutHR";
        private const string RecompiledUnsupportedChangesOnExitPlaymodeKey = "HotReloadWindow.RecompiledUnsupportedChangesOnExitPlaymode";

        public static int ServerPort {
            get { return SessionState.GetInt(ServerPortKey, RequestHelper.defaultPort); }
            set { SessionState.SetInt(ServerPortKey, value); }
        }
        
        public static string LastPatchId {
            get { return SessionState.GetString(LastPatchIdKey, string.Empty); }
            set { SessionState.SetString(LastPatchIdKey, value); }
        }
        
        public static bool ShowingRedDot {
            get { return SessionState.GetBool(ShowingRedDotKey, false); }
            set { SessionState.SetBool(ShowingRedDotKey, value); }
        }
        
        public static bool ShowedEditorsWithoutHR {
            get { return SessionState.GetBool(ShowedEditorsWithoutHRKey, false); }
            set { SessionState.SetBool(ShowedEditorsWithoutHRKey, value); }
        }
        
        public static bool RecompiledUnsupportedChangesOnExitPlaymode {
            get { return SessionState.GetBool(RecompiledUnsupportedChangesOnExitPlaymodeKey, false); }
            set { SessionState.SetBool(RecompiledUnsupportedChangesOnExitPlaymodeKey, value); }
        }
    }

}
