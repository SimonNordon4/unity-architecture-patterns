using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(AvoidAllies))]
    public class MoveTowardsTarget : MoveBase
    {
        private CombatTarget _target;
        private AvoidAllies _avoidance;
    
        
        private void Start()
        {
            _target = GetComponent<CombatTarget>();
            _avoidance = GetComponent<AvoidAllies>();
        }

        public void Update()
        {
            if (!_target.HasTarget) return;
            
            var velocity = _target.TargetDirection * Speed;
            var lookDirection = _target.TargetDirection;

            if (_target.TargetDistance < 0.75f)
            {
                velocity = Vector3.zero;
            }

            velocity += _avoidance.avoidanceDirection;
            
            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(velocity);
        }
    }
}