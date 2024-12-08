using UnityEditor;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class ScriptableData : ScriptableObject
        {
        private bool _isPlayMode;

        /// <summary>
        /// Method to define how data should reset.
        /// Override this in derived classes.
        /// </summary>
        public abstract void ResetData();

        protected void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        protected void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                _isPlayMode = true;
            }
            else if (_isPlayMode && stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                ResetData();
                _isPlayMode = false;
            }
        }
    }
}