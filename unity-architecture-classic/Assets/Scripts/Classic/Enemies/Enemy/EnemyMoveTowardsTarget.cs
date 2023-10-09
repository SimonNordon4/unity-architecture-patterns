using System;
using Classic.Interfaces;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyMoveTowardsTarget : MonoBehaviour, IEnemyMovement
    {
        public Vector3 velocity { get; private set; }
        
        [SerializeField] private float speed = 4f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float stopDistance = 0.1f;
        
        private Transform target;

        private void OnEnable()
        {
                        
        }

        private void GetClosestTarget()
        {
            
        }

        private void Update()
        {

        }
    }
}