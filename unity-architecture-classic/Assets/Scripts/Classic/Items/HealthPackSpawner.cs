using System.Collections.Generic;
using System.Linq;
using Classic.Game;
using UnityEngine;

namespace Classic.Items
{
    public class HealthPackSpawner : MonoBehaviour
    {
        [SerializeField] private Stats stats;
        [SerializeField] private HealthPackController healthPackPrefab;
        [SerializeField] private GameState gameState;

        private readonly List<HealthPackController> _healthPacks = new();

        private void OnEnable()
        {
            gameState.onGameQuit.AddListener(DestroyAllHealthPacks);
        }

        private void OnDisable()
        {
            gameState.onGameQuit.RemoveListener(DestroyAllHealthPacks);
        }

        public void SpawnHealthPack(Vector3 position)
        {
            var randomChance = Random.Range(0, 100);
            if (!(randomChance < stats.healthPackSpawnRate.value)) return;
            
            var pos = new Vector3(position.x, 0f, position.z);
            var healthPack = Instantiate(healthPackPrefab, pos, Quaternion.identity);
            healthPack.gameState = gameState;
            _healthPacks.Add(healthPack);
        }

        public void DestroyAllHealthPacks()
        {
            foreach (var healthPack in _healthPacks.Where(healthPack => healthPack != null))
            {
                Destroy(healthPack.gameObject);
            }
            _healthPacks.Clear();
        }

        private void OnValidate()
        {
            if (stats == null)
            {
                stats = FindObjectsByType<Stats>(FindObjectsSortMode.None)[0];
            }
        }
    }
}