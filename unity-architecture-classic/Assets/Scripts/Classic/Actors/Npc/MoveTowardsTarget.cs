using UnityEngine;

namespace Classic.Actors.Npc
{
    [RequireComponent(typeof(ActorMovement))]
    [RequireComponent(typeof(ActorStats))]
    [RequireComponent(typeof(ActorTarget))]
    [RequireComponent(typeof(NpcAvoidance))]
    public class MoveTowardsTarget : ActorComponent
    {
        private ActorMovement _movement;
        private ActorStats _stats;
        private Stat _moveSpeed;
        private ActorTarget _target;
        private NpcAvoidance _avoidance;

        private void Start()
        {
            _movement = GetComponent<ActorMovement>();
            _stats = GetComponent<ActorStats>();
            _target = GetComponent<ActorTarget>();
            _avoidance = GetComponent<NpcAvoidance>();
            _moveSpeed = _stats.Map[StatType.MoveSpeed];
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
            
            _movement.SetVelocityAndLookDirection(velocity, lookDirection);
        }
    }
}