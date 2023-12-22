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
        private readonly Dictionary<EnemyDefinition, Queue<PoolableEnemy>> _pools = new();
        private Action _onDeath;

        public PoolableEnemy Get(EnemyDefinition definition, Vector3 position, bool startActive = true)
        {
            if (!_pools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<PoolableEnemy>();
                _pools.Add(definition, queue);
            }

            if (queue.Count == 0)
            {
                var newEnemy = CreateEnemy(definition, position);
                newEnemy.Construct(this);
                return newEnemy;
            }

            var enemy = queue.Dequeue();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(startActive);
            
            return enemy;
        }

        public void Return(PoolableEnemy poolableEnemy, EnemyDefinition definition)
        {
            poolableEnemy.gameObject.SetActive(false);
            
            // Add the enemy to the queue
            _pools[definition].Enqueue(poolableEnemy);
        }

        private PoolableEnemy CreateEnemy(EnemyDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);

            // Because the enemy is pooled we only need to subscribe to it once.
            // We generally don't need to clean up the subscription because all enemies will destroyed before t his.
            if (!newEnemy.TryGetComponent<ActorHealth>(out var actorHealth)) return newEnemy;
            actorHealth.OnDeath += () =>
            {
                Return(newEnemy, definition);
            };
            
            return newEnemy;
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
            
            if(GUILayout.Button("Destroy Pool"))
                pool.Reset();
        }

    }
    #endif
}
