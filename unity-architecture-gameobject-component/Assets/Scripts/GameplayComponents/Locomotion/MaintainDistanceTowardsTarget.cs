 using GameplayComponents.Actor;
using GameplayComponents.Combat;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class MaintainDistanceTowardsTarget : GameplayComponent
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        [SerializeField] private AvoidAllies avoidance;

        private Stat _range;

        private void Start()
        {
            _range = stats.GetStat(StatType.RangedRange);
        }

        public void Update()
        {
            if (!target.hasTarget) return;

            var velocity = Vector3.zero;
            var lookDirection = target.targetDirection;

            if (target.targetDistance > _range.value)
            {
                velocity = target.targetDirection * _range.value;
            }
            else if (target.targetDistance < _range.value)
            {
                velocity = -target.targetDirection * _range.value;
            }

            velocity += avoidance.avoidanceDirection;

            movement.SetLookDirection(lookDirection);
            movement.AddVelocity(velocity);
        }
    }
}