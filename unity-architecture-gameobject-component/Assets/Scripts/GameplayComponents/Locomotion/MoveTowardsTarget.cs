using GameplayComponents.Actor;
using GameplayComponents.Combat;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(AvoidAllies))]
    public class MoveTowardsTarget : GameplayComponent
    {
        private Movement _movement;
        private Stats _stats;
        private Stat _moveSpeed;
        private CombatTarget _target;
        private AvoidAllies _avoidance;

        private void Start()
        {
            _movement = GetComponent<Movement>();
            _stats = GetComponent<Stats>();
            _target = GetComponent<CombatTarget>();
            _avoidance = GetComponent<AvoidAllies>();
            _moveSpeed = _stats.GetStat(StatType.MoveSpeed);
        }

        public void Update()
        {
            if (!_target.hasTarget) return;
            
            var velocity = _target.targetDirection * _moveSpeed.value;
            var lookDirection = _target.targetDirection;

            if (_target.targetDistance < 0.5f)
            {
                velocity = Vector3.zero;
            }

            velocity += _avoidance.avoidanceDirection;
            
            _movement.SetLookDirection(lookDirection);
            _movement.AddVelocity(velocity);
        }
    }
}