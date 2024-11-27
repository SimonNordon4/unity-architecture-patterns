using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(AvoidAllies))]
    public class MoveTowardsTargetWhileMaintainingDistance : MoveBase
    {
        
        private CombatTarget _target;
        private AvoidAllies _avoidance;
        private Stat _rangeState;

        [SerializeField] private float acceleration = 2f;

        private Stat _rangeStat;
        private Stat _moveStat;

        private Vector3 _lastVelocity;

        private void Start()
        {
            _rangeStat = GetComponent<Stats>().GetStat(StatType.Range);
            _target = GetComponent<CombatTarget>();
            _avoidance = GetComponent<AvoidAllies>();
        }

        public void Update()
        {
            if (!_target.HasTarget) return;

            var desiredVelocity = Vector3.zero;
            var lookDirection = _target.TargetDirection;

            if (_target.TargetDistance > _rangeStat.value)
            {
                desiredVelocity = _target.TargetDirection * _moveStat.value;
            }
            else if (_target.TargetDistance < _rangeStat.value)
            {
                desiredVelocity = -_target.TargetDirection * _moveStat.value;
            }

            desiredVelocity += _avoidance.avoidanceDirection;
            
            desiredVelocity = Vector3.Lerp(_lastVelocity, desiredVelocity, Time.deltaTime * acceleration);
            
            _lastVelocity = desiredVelocity;

            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(desiredVelocity);
        }
    }
}