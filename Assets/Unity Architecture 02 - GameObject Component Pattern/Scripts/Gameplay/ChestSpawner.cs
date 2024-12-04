using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ChestSpawner : MonoBehaviour
    {

        //TODO: Break off Chest Spawner from chest.
        [Header("Chests")]
        public float chestSpawnTime = 15f;
        private float _timeSinceLastChestSpawn = 0f;
        public Vector2 chestBounds = new(20f, 20f);
        public ChestItems tier1ChestItems;
        public ChestItems tier2ChestItems;
        public ChestItems tier3ChestItems;
        public ChestItems tier4ChestItems;
        public ChestItems[] allChestItems;
        public Chest chestPrefab;
        public Chest currentChest = null;

        [Header("Pity")]
        public int tier3Pity;
        public int tier4Pity;

        public void Start()
        {
            allChestItems = new[] { tier1ChestItems, tier2ChestItems, tier3ChestItems, tier4ChestItems };

            SpawnChest();
        }

        public void ReduceChestSpawnTime()
        {
            _timeSinceLastChestSpawn += 1f;
        }

        private Vector3 GetRandomChestSpawn()
        {
            return new Vector3(Random.Range(-chestBounds.x, chestBounds.x), 0f, Random.Range(-chestBounds.y, chestBounds.y));
        }

        private void SpawnChest()
        {
            var chest = Instantiate(chestPrefab, GetRandomChestSpawn(), Quaternion.identity);
            chest.minTier = 1;
            chest.maxTier = 5;
            chest.GenerateItems();
        }

        public void SpawnBossChest(Vector3 position)
        {
            var groundPosition = new Vector3(position.x, 0, position.z);
            var chest = Instantiate(chestPrefab, groundPosition, Quaternion.identity);
            chest.minTier = 2;
            chest.maxTier = 4;
            chest.transform.localScale *= 1.5f; // Make the boss chest 50% larger
            chest.GenerateItems();
        }
    }
}
