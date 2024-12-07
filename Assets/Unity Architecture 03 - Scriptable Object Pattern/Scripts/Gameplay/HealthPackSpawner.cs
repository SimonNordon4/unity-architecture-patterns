using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class HealthPackSpawner : MonoBehaviour
    {
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private HealthPack healthPackPrefab;
        
        private readonly List<HealthPack> _healthPacks = new();
        
        public void SpawnHealthPack(Vector3 position)
        {
            var randomChance = Random.Range(0, 100);
            if (!(randomChance < 3)) return;
            
            var pos = new Vector3(position.x, 0f, position.z);
            var healthPack = Instantiate(healthPackPrefab, pos, Quaternion.identity);
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
    }
}