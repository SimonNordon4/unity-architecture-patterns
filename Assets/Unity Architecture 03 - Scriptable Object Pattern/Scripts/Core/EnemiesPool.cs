using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class EnemiesPool : ScriptableObject
    {
        [SerializeField] private Level level;

        [SerializeField] private ActorPool normalEnemyPool;
        [SerializeField] private ActorPool fastEnemyPool;
        [SerializeField] private ActorPool bigEnemyPool;
        [SerializeField] private ActorPool chargerEnemyPool;
        [SerializeField] private ActorPool rangedEnemyPool;
        [SerializeField] private ActorPool wandererEnemyPool;
        [SerializeField] private ActorPool wandererRangedEnemyPool;
        [SerializeField] private ActorPool wandererExploderEnemyPool;

        [SerializeField] private ActorPool normalBossEnemyPool;
        [SerializeField] private ActorPool fastBossEnemyPool;
        [SerializeField] private ActorPool bigBossEnemyPool;
        [SerializeField] private ActorPool chargerBossEnemyPool;
        [SerializeField] private ActorPool rangedBossEnemyPool;
        [SerializeField] private ActorPool wandererBossEnemyPool;
        [SerializeField] private ActorPool wandererRangedBossEnemyPool;
        [SerializeField] private ActorPool wandererExploderBossEnemyPool;

        private Dictionary<EnemyType, ActorPool> _poolMap;
        private Dictionary<EnemyType, ActorPool> _bossMap;

        private void OnEnable()
        {
            _poolMap = new Dictionary<EnemyType, ActorPool>
            {
                { EnemyType.Normal, normalEnemyPool },
                { EnemyType.Fast, fastEnemyPool },
                { EnemyType.Big, bigEnemyPool },
                { EnemyType.Charger, chargerEnemyPool },
                { EnemyType.Ranged, rangedEnemyPool },
                { EnemyType.Wanderer, wandererEnemyPool },
                { EnemyType.WandererRanged, wandererRangedEnemyPool },
                { EnemyType.WandererExploder, wandererExploderEnemyPool }
            };

            _bossMap = new Dictionary<EnemyType, ActorPool>
            {
                { EnemyType.Normal, normalBossEnemyPool },
                { EnemyType.Fast, fastBossEnemyPool },
                { EnemyType.Big, bigBossEnemyPool },
                { EnemyType.Charger, chargerBossEnemyPool },
                { EnemyType.Ranged, rangedBossEnemyPool },
                { EnemyType.Wanderer, wandererBossEnemyPool },
                { EnemyType.WandererRanged, wandererRangedBossEnemyPool },
                { EnemyType.WandererExploder, wandererExploderBossEnemyPool }  
            };
        }


        public GameObject GetEnemyByType(EnemyType enemyType)
        {
            GameObject enemy = null;

            var randomPosition = new Vector3(
                Random.Range(-level.Bounds.x, level.Bounds.x),
                0f,
                Random.Range(-level.Bounds.y, level.Bounds.y)
            );

            if (_poolMap.TryGetValue(enemyType, out ActorPool pool))
            {
                enemy = pool.Get(randomPosition);
                enemy.transform.position = randomPosition;
                enemy.gameObject.SetActive(true);
            }

            return enemy;
        }

        public GameObject GetBossByType(EnemyType enemyType)
        {
            GameObject enemy = null;

            var randomPosition = new Vector3(
                Random.Range(-level.Bounds.x, level.Bounds.x),
                0f,
                Random.Range(-level.Bounds.y, level.Bounds.y)
            );

            if (_bossMap.TryGetValue(enemyType, out ActorPool pool))
            {
                enemy = pool.Get(randomPosition);
                enemy.transform.position = randomPosition;
                enemy.gameObject.SetActive(true);
            }

            return enemy;
        }
    }
}