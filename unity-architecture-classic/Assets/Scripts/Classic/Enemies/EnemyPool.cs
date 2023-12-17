using System;
using System.Collections.Generic;
using Classic.Actors;
using Classic.Game;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Enemies
{
    public class EnemyPool : ActorComponent
    {
        [field:SerializeField] public EnemyFactory factory { get; private set; }
        private readonly List<Enemy> _activeEnemies = new();
        private readonly Dictionary<EnemyDefinition, Queue<Enemy>> _pools = new();
        private Action _onDeath;

        public Enemy Get(EnemyDefinition definition, Vector3 position, bool startActive = true)
        {
            if (!_pools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<Enemy>();
                _pools.Add(definition, queue);
            }

            if (queue.Count == 0)
            {
                return CreateEnemy(definition, position);
            }

            var enemy = queue.Dequeue();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(startActive);
            
            _activeEnemies.Add(enemy);
            
            return enemy;
        }

        public  void Return(Enemy enemy, EnemyDefinition definition)
        {
            //Reset the enemy
            if(enemy.TryGetComponent<ActorState>(out var state))
                state.ResetActor();
            
            enemy.gameObject.SetActive(false);
            
            // Remove the enemy from the active list
            _activeEnemies.Remove(enemy);
            
            // Add the enemy to the queue
            _pools[definition].Enqueue(enemy);
        }

        private Enemy CreateEnemy(EnemyDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);

            // Because the enemy is pooled we only need to subscribe to it once.
            // We generally don't need to clean up the subscription because all enemies will destroyed before this.
            if (!newEnemy.TryGetComponent<ActorHealth>(out var actorHealth)) return newEnemy;
            actorHealth.OnDeath += () => Return(newEnemy, definition);
            
            return newEnemy;
        }
        
        public override void Reset()
        {
            Debug.Log("Resetting enemy pool.");
            DestroyPool();
        }

        private void DestroyPool()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var enemy = pool.Dequeue();
                    Destroy(enemy.gameObject);
                }
            }
            _pools.Clear();
            
            foreach (var enemy in _activeEnemies)
            {
                Destroy(enemy.gameObject);
            }
            _activeEnemies.Clear();
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(EnemyPool))]
    public class EnemyPoolEditor : Editor
    {
        private EnemyDefinition enemyDefinition;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var pool = (EnemyPool)target;

            enemyDefinition =
                (EnemyDefinition)EditorGUILayout.ObjectField(enemyDefinition, typeof(EnemyDefinition), false);

            if (GUILayout.Button("Create Enemy"))
            {
                pool.Get(enemyDefinition, Vector3.zero);
            }
        }

    }
    #endif
}
