using UnityEditor;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class ScriptableData : ScriptableObject
    {
        protected bool IsPlayMode;

        public void OnDestroy()
        {
            Debug.Log($"Destroyed {GetType()}");
        }

        /// <summary>
        /// Method to define how data should reset.
        /// Override this in derived classes.
        /// </summary>
        public abstract void ResetData();

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                IsPlayMode = true;
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                IsPlayMode = true;
            }
            else if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                // Avoid resetting data unnecessarily
                Debug.Log("Exiting edit mode, skipping ResetData.");
            }
            else if (IsPlayMode && stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("ResetData called for: " + name);
                ResetData();
                IsPlayMode = false;
            }
        }
#endif

    }
}