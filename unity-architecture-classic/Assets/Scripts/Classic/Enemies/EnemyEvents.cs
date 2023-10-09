using System;
using Classic.Enemies.Enemy;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyEvents : MonoBehaviour
    {
        public Action<EnemyScope, Vector3> onEnemySpawned;
        public Action<EnemyScope, Vector3> onEnemyDied;
        public Action<int,Vector3> onEnemyDamaged;
    }
}