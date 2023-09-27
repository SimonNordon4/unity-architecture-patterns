using System;
using Classic.Core;
using UnityEngine;

namespace Classic.UI
{
    public class UIPauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameState gameState;

        /// <summary>
        /// This is potentially an issue as we're never unsubscribing from the events.
        /// </summary>
        private void Start()
        {
            // The pause menu will only enable if the currentState is the paused state.
            gameState.onStateChanged.AddListener(() => 
                pauseMenu.SetActive(gameState.currentState == GameStateEnum.Paused));
            pauseMenu.SetActive(false);
        }
    }
}