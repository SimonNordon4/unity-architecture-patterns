using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(ChestSpawner))]
    public class ChestPickupHandler : MonoBehaviour
    {
        public UnityEvent OnChestPickup = new();
        
        [SerializeField]private GameState gameState;
        
        private ChestSpawner _chestSpawner;
        public Chest CurrentChest { get; private set; } = null;


        private void Awake()
        {
            _chestSpawner= GetComponent<ChestSpawner>();
        }

        private void OnEnable()
        {
            _chestSpawner.OnChestSpawned.AddListener(OnChestSpawned);
        }

        private void OnChestSpawned(Chest chest)
        {
            chest.OnChestOpened.AddListener(PickupChest);
        }

        private void PickupChest(Chest chest)
        {
            chest.OnChestOpened.RemoveListener(PickupChest);
            gameState.PauseGame();
            OnChestPickup.Invoke();
        }

    }
}