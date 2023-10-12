using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyMoveTowards : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        [SerializeField] private EnemyStats stats;
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private float repulsionForce = 1f;
        [SerializeField] private EnemyAvoidance avoidance;
        
        private void LateUpdate()
        {
            var characterPosition = scope.characterTransform.position;
            var enemyPosition = enemyTransform.position;
            var dir =  Vector3.ProjectOnPlane(characterPosition - enemyPosition,Vector3.up).normalized;
            var distance = Vector3.Distance(characterPosition, enemyPosition);
            if (distance < stats.attackRange)
            {
                dir = Vector3.zero;
            }
            
            var updatedDir = (dir + avoidance.avoidanceDirection * repulsionForce).normalized;

            // Apply direction to transform.
            if (!(dir.magnitude > 0))
            {
                stats.velocity = Vector3.zero;
                return;
            }
            
            stats.velocity = updatedDir * stats.moveSpeed;
            transform.position += stats.velocity * GameTime.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}