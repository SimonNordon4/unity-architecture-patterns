using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.App;
using GameplayComponents.Combat;
using GameObjectComponent.Game;
using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Items;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class HealthPackSpawner : GameplayComponent
    {
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private Stats stats;
        [SerializeField] private HealthPack healthPackPrefab;
        
        private Stat _healthPackDropRate;
        
        private readonly List<HealthPack> _healthPacks = new();

        private void Start()
        {
            _healthPackDropRate = stats.GetStat(StatType.HealthPackDropRate);
        }
        
        public void SpawnHealthPack(Vector3 position)
        {
            
            var randomChance = Random.Range(0, 100);
            if (!(randomChance < _healthPackDropRate.value)) return;
            
            var pos = new Vector3(position.x, 0f, position.z);
            var healthPack = Instantiate(healthPackPrefab, pos, Quaternion.identity);
            if(healthPack.TryGetComponent<SoundProxy>(out var soundProxy))
                soundProxy.Construct(soundManager);
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

        public override void OnGameEnd()
        {
            DestroyAllHealthPacks();
        }
    }
}