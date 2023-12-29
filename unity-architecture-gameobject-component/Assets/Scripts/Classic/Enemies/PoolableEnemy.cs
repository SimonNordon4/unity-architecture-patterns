using System;
using Classic.Actors;
using Classic.Pools;
using UnityEngine;

namespace Classic.Enemies
{
    public class PoolableEnemy : ActorComponent
    {
        [SerializeField]private EnemyDefinition definition;
        private EnemyPool _pool;
        
        public void Construct(EnemyPool pool)
        {
            _pool = pool;
        }
        
        public override void Reset()
        {
            _pool.Return(this, definition);
        }
    }
}