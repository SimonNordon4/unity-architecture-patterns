using System;
using System.Collections.Generic;
using Classic.Enemies.Enemy;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [SerializeField] private Transform characterTransform;

        private readonly Dictionary<EnemyType, Queue<EnemyScope>> _enemyQueues = new();

        public void Initialize()
        {
            foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
            {
                _enemyQueues[type] = new Queue<EnemyScope>();
            }
        }

        private void Start()
        {
            Initialize();
        }

        public EnemyScope Spawn(EnemyDefinition enemyDefinition, Vector3 position)
        {
            var queue = _enemyQueues[enemyDefinition.enemyType];
            if (queue.Count == 0)
            {
                return Create(enemyDefinition, position);
            }

            var enemyScope = queue.Dequeue();
            enemyScope.transform.position = position;
            enemyScope.gameObject.SetActive(true);
            return enemyScope;
        }

        public void Return(EnemyScope returningEnemy)
        {
            returningEnemy.gameObject.SetActive(false);
            _enemyQueues[returningEnemy.type].Enqueue(returningEnemy);
        }

        private EnemyScope Create(EnemyDefinition enemyDefinition, Vector3 position = new())
        {
            var enemyScope = Instantiate(enemyDefinition.enemyPrefab, position, Quaternion.identity, null);
            enemyScope.Construct(state, level, characterTransform);
            
            if(enemyScope.TryGetComponent<EnemyStats>(out var enemyStats))
            {
                enemyStats.Initialize(enemyDefinition);
            }
            
            if (enemyScope.type != enemyDefinition.enemyType)
            {
                Debug.LogError("EnemyScope type does not match EnemyDefinition type");
            }
            return enemyScope;
        }
    }
}