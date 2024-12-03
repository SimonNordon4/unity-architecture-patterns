using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class EnemySpawner : MonoBehaviour
    {
        public UnityEvent<PoolableActor> OnEnemyDied = new();
        public UnityEvent<PoolableActor> OnBossDied = new();

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
            
            normalEnemyPool.OnActorReturn.AddListener(EnemyDied);
            fastEnemyPool.OnActorReturn.AddListener(EnemyDied);
            bigEnemyPool.OnActorReturn.AddListener(EnemyDied);
            chargerEnemyPool.OnActorReturn.AddListener(EnemyDied);
            rangedEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererRangedEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererExploderEnemyPool.OnActorReturn.AddListener(EnemyDied);
            
            normalBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            fastBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            bigBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            chargerBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            rangedBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererRangedBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
            wandererExploderBossEnemyPool.OnActorReturn.AddListener(EnemyDied);
        }

        private void OnDisable()
        {
            normalEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            fastEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            bigEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            chargerEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            rangedEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererRangedEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererExploderEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            
            normalBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            fastBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            bigBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            chargerBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            rangedBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererRangedBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
            wandererExploderBossEnemyPool.OnActorReturn.RemoveListener(EnemyDied);
        }

        public void EnemyDied(PoolableActor actor)
        {
            OnEnemyDied?.Invoke(actor);
        }
        
        public void BossDied(PoolableActor actor)
        {
            OnBossDied?.Invoke(actor);
        }

        public void SpawnEnemy(EnemyType enemyType)
        {
            PoolableActor enemy = null;

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
        }

        public void SpawnBoss(EnemyType enemyType)
        {
            PoolableActor enemy = null;

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
        }
    }
}