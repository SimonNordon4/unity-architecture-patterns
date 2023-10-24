using System.Collections.Generic;
using Classic.Actors;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField]private EnemyFactory factory;
        private readonly Dictionary<EnemyDefinition, Queue<Enemy>> _pools = new();

        public Enemy Get(EnemyDefinition enemyDefinition, Vector3 position, bool startActive = true)
        {
            // check if the definition is in the dictionary
            if (!_pools.ContainsKey(enemyDefinition))
            {
                // if not, create a new queue
                _pools.Add(enemyDefinition, new Queue<Enemy>());
            }
            
            // check if the queue is empty
            if (_pools[enemyDefinition].Count == 0)
            {
                // if so, create a new enemy
                return factory.Create(enemyDefinition, position).GetComponent<Enemy>();
            }
            
            // if not, get the enemy from the queue
            var enemy = _pools[enemyDefinition].Dequeue();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(startActive);
            return enemy;
        }
        
        public void Return(Enemy enemy)
        {
            // get the enemy definition
            var enemyDefinition = enemy.enemyDefinition;
            
            // reset the enemy
            if(TryGetComponent<ActorState>(out var state))
                state.ResetActor();
            
            // add the enemy to the queue
            _pools[enemyDefinition].Enqueue(enemy);
        }
    }
}