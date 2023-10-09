using System;
using System.Collections.Generic;
using Classic.Enemies.Enemy;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private List<EnemyDefinition> enemyDefinitions = new();
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [SerializeField] private EnemyEvents enemyEvents;
        private readonly Dictionary<EnemyDefinition, Queue<EnemyScope>> enemyMap = new();

        private void Start()
        {
            // Create a queue for each enemy definition
            foreach (var enemyDefinition in enemyDefinitions)
            {
                enemyMap.Add(enemyDefinition, new Queue<EnemyScope>());
            }
        }

        public EnemyScope Spawn(EnemyDefinition enemyDefinition, Vector3 location, bool isBoss = false)
        {
            var queue = enemyMap[enemyDefinition];
            if (queue.Count <= 0)
            {
                return EnemyFactory(enemyDefinition,location,isBoss);
            }
            
            var enemy = queue.Dequeue();
            enemy.gameObject.SetActive(true);
            enemy.transform.position = location;
            return enemy;
        }

        public void DeSpawn(EnemyScope enemyState)
        {
            enemyState.gameObject.SetActive(false);
            enemyMap[enemyState.definition].Enqueue(enemyState);
        }

        private EnemyScope EnemyFactory(EnemyDefinition definition, Vector3 location, bool isBoss)
        {
            var newEnemy = Instantiate(definition.enemyPrefab, location, Quaternion.identity, null);
            newEnemy.Construct(state,level,enemyEvents,isBoss);
            if (newEnemy.TryGetComponent<EnemyPoolable>(out var poolable))
            {
                poolable.pool = this;
            }
            return newEnemy;
        }
    }
}