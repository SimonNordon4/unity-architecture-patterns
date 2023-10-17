using UnityEngine;

namespace Classic.Actors.Npc
{
    [RequireComponent(typeof(ActorMovement))]
    [RequireComponent(typeof(ActorStats))]
    [RequireComponent(typeof(ActorTarget))]
    public class MoveTowardsTarget : ActorComponent
    {
        private ActorMovement _movement;
        private ActorStats _stats;
        private Stat _moveSpeed;
        private ActorTarget _target;

        private void Start()
        {
            _movement = GetComponent<ActorMovement>();
            _stats = GetComponent<ActorStats>();
            _target = GetComponent<ActorTarget>();
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
            
            _movement.SetVelocityAndLookDirection(velocity, lookDirection);
        }
    }
}