using UnityEditor;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class ScriptableData : ScriptableObject
    {
        protected bool IsPlayMode;

        /// <summary>
        /// Method to define how data should reset.
        /// Override this in derived classes.
        /// </summary>
        public abstract void ResetData();

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                IsPlayMode = true;
            }
            else if (IsPlayMode && stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                ResetData();
                IsPlayMode = false;
            }
        }
    }
}