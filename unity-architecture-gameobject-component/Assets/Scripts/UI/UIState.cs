﻿using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectComponent.UI
{
    public class UIState : MonoBehaviour
    {
        [SerializeField]private GameState gameState;
        
        private readonly Stack<UIStateEnum> _previousStates = new();
        [field: SerializeField]
        public UIStateEnum currentState { get; private set; } = UIStateEnum.MainMenu;
        public UnityEvent<UIStateEnum> onStateChanged { get; } = new();

        private void Start()
        {
            GoToMainMenu();
        }

        private void OnEnable()
        {
            gameState.OnGameStart+=(GoToHud);
            gameState.OnGamePause+=(GoToPauseMenu);
            gameState.OnGameResume+=(GoToHud);
            gameState.OnGameWon+=(GoToGameWonMenu);
            gameState.OnGameLost+=(GoToGameOverMenu);
            gameState.OnGameQuit+=(GoToMainMenu);

        }

        private void OnDisable()
        {
            gameState.OnGameStart-=(GoToHud);
            gameState.OnGamePause-=(GoToPauseMenu);
            gameState.OnGameResume-=(GoToHud);
            gameState.OnGameWon-=(GoToGameWonMenu);
            gameState.OnGameLost-=(GoToGameOverMenu);
            gameState.OnGameQuit-=(GoToMainMenu);
        }

        private void OnDestroy()
        {
            _previousStates.Clear();
            onStateChanged.RemoveAllListeners();
        }
        
        public void GoToState(UIStateEnum state)
        {
            _previousStates.Push(currentState);
            currentState = state;
            onStateChanged.Invoke(currentState);
        }
        
        public void GoToPreviousState()
        {
            if (_previousStates.Count <= 0) return;
            currentState = _previousStates.Pop();
            onStateChanged.Invoke(currentState);
        }


        public void GoToMainMenu()
        {
            GoToState(UIStateEnum.MainMenu);
        }
        
        public void GoToHud()
        {
            GoToState(UIStateEnum.Hud);
        }

        public void GoToPauseMenu()
        {
            GoToState(UIStateEnum.PauseMenu);
        }

        public void GoToStore()
        {
            GoToState(UIStateEnum.Store);
        }
        
        public void GoToAchievements()
        {
            GoToState(UIStateEnum.Achievements);
        }
        
        public void GoToSettings()
        {
            GoToState(UIStateEnum.Settings);
        }
        
        public void GoToGameOverMenu()
        {
            GoToState(UIStateEnum.GameOver);
        }
        
        public void GoToGameWonMenu()
        {
            GoToState(UIStateEnum.GameWon);
        }

        public void GoToStatistics()
        {
            GoToState(UIStateEnum.Statistics);
        }
        
        public void GoToChestMenu()
        {
            GoToState(UIStateEnum.ChestMenu);
        }
    }
    
    public enum UIStateEnum
    {
        MainMenu,
        PauseMenu,
        Hud,
        GameOver,
        GameWon,
        Store,
        Achievements,
        Statistics,
        Settings,
        ChestMenu
        
    }
}