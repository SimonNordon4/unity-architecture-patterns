using System.Collections;
using System.Collections.Generic;
using Classic.Enemies.Enemy;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyScope enemy;
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [SerializeField] private Transform characterTransform;
        
        private readonly Queue<EnemyScope> _queue = new();

        public EnemyScope Spawn(Vector3 position)
        {
            if (_queue.Count == 0)
            {
                return Create(position);
            }
            
            var enemyScope = _queue.Dequeue();
            enemyScope.transform.position = position;
            enemyScope.gameObject.SetActive(true);
            return enemyScope;
        }


        public void Return(EnemyScope returningEnemy)
        {
            returningEnemy.gameObject.SetActive(false);
            _queue.Enqueue(returningEnemy);
        }

        public EnemyScope Create(Vector3 position = new())
        {
            var enemyScope = Instantiate(enemy, position, Quaternion.identity, null);
            enemyScope.Construct(state,level,characterTransform);
            return enemyScope;
        }
    }
}