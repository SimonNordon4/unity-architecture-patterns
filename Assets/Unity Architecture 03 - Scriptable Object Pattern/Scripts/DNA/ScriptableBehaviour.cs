﻿using System;
using UnityEngine;
using UnityEngine.LowLevel;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class ScriptableBehaviour : ScriptableData
    {
        private bool _isActive;

        /// <summary>
        /// Update logic to be implemented by derived classes.
        /// </summary>
        public abstract void OnUpdate();

        private void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        private void OnDisable()
        {
            base.OnDisable();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
            if (_isActive)
            {
                DisablePlayerLoop();
            }
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
        {
            if (stateChange == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            {
                EnablePlayerLoop();
            }
            else if (stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                DisablePlayerLoop();
            }
        }
#endif

        private void EnablePlayerLoop()
        {
            if (_isActive) return;

            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem updateSystem = new PlayerLoopSystem
            {
                type = typeof(ScriptableBehaviour),
                updateDelegate = () =>
                {
                    if (Application.isPlaying) OnUpdate();
                }
            };

            var systems = new System.Collections.Generic.List<PlayerLoopSystem>(playerLoop.subSystemList)
            {
                updateSystem
            };
            playerLoop.subSystemList = systems.ToArray();
            PlayerLoop.SetPlayerLoop(playerLoop);

            _isActive = true;
        }

        private void DisablePlayerLoop()
        {
            if (!_isActive) return;

            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList = Array.FindAll(playerLoop.subSystemList,
                system => system.type != typeof(ScriptableBehaviour));
            PlayerLoop.SetPlayerLoop(playerLoop);

            _isActive = false;
        }
    }
}
