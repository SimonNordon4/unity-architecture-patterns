using GameObjectComponent.Game;
using GameplayComponents;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class RoundMiniChestSpawner : GameplayComponent
    {
        [SerializeField] private ChestSpawner chestSpawner;
        [SerializeField] private float miniChestSpawnRate = 8f;
        
        private float _timeSinceLastChest = 0.0f;
        private bool _isChestSpawnAllowed = true;
        private Chest _currentChest = null;

        private void Update()
        {
            if (!_isChestSpawnAllowed) return;

            if (_timeSinceLastChest < miniChestSpawnRate)
            {
                _timeSinceLastChest += GameTime.deltaTime;
                return;
            }
            
            SpawnMiniChest();
            _isChestSpawnAllowed = false;
        }

        private void SpawnMiniChest()
        {
            Debug.Log("Spawning Mini Chest");
            _currentChest = chestSpawner.SpawnMiniChest();
            _currentChest.onPickedUp.AddListener(ChestPickedUp);
        }

        private void ChestPickedUp()
        {
            _currentChest.onPickedUp.RemoveListener(ChestPickedUp);
            _isChestSpawnAllowed = true;
            _timeSinceLastChest = 0f;
        }
        
        public override void OnGameEnd()
        {
            _isChestSpawnAllowed = false;
            _timeSinceLastChest = miniChestSpawnRate;
        }
    }
}