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
        public event Action<PoolableEnemy> OnEnemySpawned;
        public event Action<PoolableEnemy> OnEnemyDespawned;

        public PoolableEnemy Get(EnemyDefinition definition, Vector3 position, bool startActive = true)
        {
            if (!_pools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<PoolableEnemy>();
                _pools.Add(definition, queue);
            }
            
            PoolableEnemy enemy = null;

            if (queue.Count == 0)
            {
                enemy = CreateEnemy(definition, position);
                enemy.Construct(this);
                enemy.transform.position = position;
                enemy.gameObject.SetActive(startActive);
            }
            else
            {
                enemy = queue.Dequeue();
                enemy.transform.position = position;
                enemy.gameObject.SetActive(startActive);
            }
            
            OnEnemySpawned?.Invoke(enemy);
            
            return enemy;
        }

        public void Return(PoolableEnemy poolableEnemy, EnemyDefinition definition)
        {
            poolableEnemy.gameObject.SetActive(false);
            
            OnEnemyDespawned?.Invoke(poolableEnemy);
            
            // Add the enemy to the queue
            _pools[definition].Enqueue(poolableEnemy);
        }

        private PoolableEnemy CreateEnemy(EnemyDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);
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
