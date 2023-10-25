using System;
using System.Collections.Generic;
using Classic.Actors;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Enemies
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyFactory factory;
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
            return enemy;
        }

        private void Return(Enemy enemy, EnemyDefinition definition)
        {
            // Reset the enemy
            if(enemy.TryGetComponent<ActorState>(out var state))
                state.ResetActor();
            
            enemy.gameObject.SetActive(false);
            
            // Add the enemy to the queue
            _pools[definition].Enqueue(enemy);
        }

        private Enemy CreateEnemy(EnemyDefinition definition, Vector3 position)
        {
            var newEnemy = factory.Create(definition, position);

            if (!newEnemy.TryGetComponent<ActorHealth>(out var actorHealth)) return newEnemy;
            
            newEnemy.onDeath = () => Return(newEnemy, definition);
            actorHealth.OnDeath += newEnemy.onDeath;

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
        }

    }
    #endif
}
