using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class MoveTowardsTarget : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        [SerializeField] private AvoidAllies avoidance;

        private Stat _moveSpeed;
        
        private void Start()
        {
            _moveSpeed = stats.GetStat(StatType.Speed);
        }

        public void Update()
        {
            if (!target.HasTarget) return;
            
            var velocity = target.TargetDirection * _moveSpeed.value;
            var lookDirection = target.TargetDirection;

            if (target.TargetDistance < 0.5f)
            {
                velocity = Vector3.zero;
            }

            velocity += avoidance.avoidanceDirection;
            
            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(velocity);
        }
    }
}