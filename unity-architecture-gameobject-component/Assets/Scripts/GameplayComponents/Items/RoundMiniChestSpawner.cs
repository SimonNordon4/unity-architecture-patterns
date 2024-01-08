using GameObjectComponent.Game;
using GameplayComponents;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class RoundMiniChestSpawner : GameplayComponent
    {
        [SerializeField] private ChestSpawner chestSpawner;
        [SerializeField] private Level level;
        [SerializeField] private Vector2 edgeBuffer = new Vector2(2f, 2f);
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
            // get a random location in the level minus the buffer
            var randomX = Random.Range(edgeBuffer.x - level.bounds.x, level.bounds.x - edgeBuffer.x);
            var randomZ = Random.Range(edgeBuffer.y - level.bounds.y, level.bounds.y - edgeBuffer.y);
            var chestPosition = new Vector3(randomX, 0f, randomZ);
            _currentChest = chestSpawner.SpawnChest(ChestType.Mini, chestPosition);
            _currentChest.onPickedUp.AddListener(ChestPickedUp);
        }

        private void ChestPickedUp()
        {
            _currentChest.onPickedUp.RemoveListener(ChestPickedUp);
            _isChestSpawnAllowed = true;
            _timeSinceLastChest = 0f;
        }
        
        public override void OnGameStart()
        {
            _isChestSpawnAllowed = true;
            _timeSinceLastChest = 0f;
        }
        
        public override void OnGameEnd()
        {
            _isChestSpawnAllowed = false;
            _timeSinceLastChest = 0f;
        }
    }
}