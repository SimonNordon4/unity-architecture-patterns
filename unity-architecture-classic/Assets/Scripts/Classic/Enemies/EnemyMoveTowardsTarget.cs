using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyMoveTowardsTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Level level;
        [SerializeField] private float speed = 4f;
        [SerializeField]private EnemyMovement movement;

        private NeighbourAvoidance _neighbourAvoidance = new();
        
        private void Update()
        {
            var dir = target.position - transform.position;
            dir.y = 0;
            movement.moveDirection = dir.normalized;
            movement.lookDirection = dir.normalized;
            movement.speed = speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}