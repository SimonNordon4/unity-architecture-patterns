using GameplayComponents.Actor;
using GameplayComponents.Combat;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class MoveTowardsTarget : GameplayComponent
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        [SerializeField] private AvoidAllies avoidance;

        private Stat _moveSpeed;
        
        private void Start()
        {
            _moveSpeed = stats.GetStat(StatType.MoveSpeed);
        }

        public void Update()
        {
            if (!target.hasTarget) return;
            
            var velocity = target.targetDirection * _moveSpeed.value;
            var lookDirection = target.targetDirection;

            if (target.targetDistance < 0.5f)
            {
                velocity = Vector3.zero;
            }

            velocity += avoidance.avoidanceDirection;
            
            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(velocity);
        }
    }
}