using System;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyEvents : MonoBehaviour
    {
        public Action<Vector3,int> OnEnemyDamaged;
        public Action<Vector3> OnEnemyDeath; 
    }
}