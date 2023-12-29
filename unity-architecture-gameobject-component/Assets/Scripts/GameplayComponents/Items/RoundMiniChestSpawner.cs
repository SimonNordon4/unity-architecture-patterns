using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class RoundMiniChestSpawner : MonoBehaviour
    {
        [SerializeField] private ChestSpawner chestSpawner;

        [SerializeField] private float miniChestSpawnRate = 8f;
        private float _timeSinceLastChest = 0.0f;
        private bool _isChestSpawnAllowed = true;

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
            var chest = chestSpawner.SpawnMiniChest();
            chest.onPickedUp.AddListener(ResetChestTimer);
        }

        private void ResetChestTimer()
        {
            _isChestSpawnAllowed = true;
            _timeSinceLastChest = 0f;
        }
    }
}