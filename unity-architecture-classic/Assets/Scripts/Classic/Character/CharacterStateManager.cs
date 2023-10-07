using System.Collections;
using System.Collections.Generic;
using Classic.Game;
using Classic.Interfaces;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterStateManager : MonoBehaviour
    {
        [SerializeField] private GameState state;
        /// <summary>
        /// List of character gameobjects the should only execute when the game state is active.
        /// </summary>
        [SerializeField] private List<GameObject> characterObjects = new();
        /// <summary>
        /// Array of Character behaviours that can be reset when the game starts.
        /// </summary>
        private IResettable[] _resettables;

        private void OnEnable()
        {
            state.onGameStart.AddListener(ResetCharacter);
            state.onStateChanged.AddListener(ProcessGameState);
        }

        private void OnDisable()
        {
            state.onGameStart.RemoveListener(ResetCharacter);
            state.onStateChanged.RemoveListener(ProcessGameState);
        }

        private void Start()
        {
            _resettables = GetComponentsInChildren<IResettable>();
        }

        private void ProcessGameState()
        {
            foreach (var characterObject in characterObjects)
            {
                characterObject.SetActive(state.isGameActive);
            }
        }
        
        private void ResetCharacter()
        {
            ProcessGameState();
            foreach (var resettable in _resettables)
            {
                resettable.Reset();
            }
        }
    }    
}

