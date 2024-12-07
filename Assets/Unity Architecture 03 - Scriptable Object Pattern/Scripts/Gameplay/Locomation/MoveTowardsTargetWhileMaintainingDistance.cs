using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(AvoidAllies))]
    public class MoveTowardsTargetWhileMaintainingDistance : MoveBase
    {
        
        private CombatTarget _target;
        private AvoidAllies _avoidance;

        [SerializeField] private float acceleration = 2f;

        private Stat _rangeStat;
        private Stat _speedStat;

        private Vector3 _lastVelocity;

        private void Start()
        {
            var stats = GetComponent<Stats>();
            _rangeStat = stats.GetStat(StatType.Range);
            _speedStat = stats.GetStat(StatType.Speed);
            _target = GetComponent<CombatTarget>();
            _avoidance = GetComponent<AvoidAllies>();
        }

        public void Update()
        {
            if (!_target.HasTarget) return;

            var desiredVelocity = Vector3.zero;
            var lookDirection = _target.TargetDirection;

            if (_target.TargetDistance > _rangeStat.value * 0.5f)
            {
                desiredVelocity = _target.TargetDirection * Speed;
            }
            else if (_target.TargetDistance < _rangeStat.value * 0.5f - 1)
            {
                desiredVelocity = -_target.TargetDirection * Speed;
            }

            desiredVelocity += _avoidance.avoidanceDirection;
            
            desiredVelocity = Vector3.Lerp(_lastVelocity, desiredVelocity, Time.deltaTime * acceleration);
            
            _lastVelocity = desiredVelocity;

            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(desiredVelocity);
        }
    }
}