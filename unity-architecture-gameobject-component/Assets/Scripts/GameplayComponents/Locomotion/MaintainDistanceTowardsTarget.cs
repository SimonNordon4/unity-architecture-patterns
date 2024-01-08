using GameObjectComponent.Game;
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

        [SerializeField] private float acceleration = 2f;

        private Stat _rangeStat;
        private Stat _moveStat;

        private Vector3 _lastVelocity;

        private void Start()
        {
            _rangeStat = stats.GetStat(StatType.RangedRange);
            _moveStat = stats.GetStat(StatType.MoveSpeed);
        }

        public void Update()
        {
            if (!target.hasTarget) return;

            var desiredVelocity = Vector3.zero;
            var lookDirection = target.targetDirection;

            if (target.targetDistance > _rangeStat.value)
            {
                desiredVelocity = target.targetDirection * _moveStat.value;
            }
            else if (target.targetDistance < _rangeStat.value)
            {
                desiredVelocity = -target.targetDirection * _moveStat.value;
            }

            desiredVelocity += avoidance.avoidanceDirection;
            
            desiredVelocity = Vector3.Lerp(_lastVelocity, desiredVelocity, GameTime.deltaTime * acceleration);
            
            _lastVelocity = desiredVelocity;

            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(desiredVelocity);
        }
    }
}