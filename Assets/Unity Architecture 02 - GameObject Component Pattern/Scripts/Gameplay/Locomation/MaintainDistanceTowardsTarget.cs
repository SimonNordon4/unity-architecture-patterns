using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class MoveTowardsTargetWhileMaintainingDistance : MonoBehaviour
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
            _rangeStat = stats.GetStat(StatType.Range);
            _moveStat = stats.GetStat(StatType.Speed);
        }

        public void Update()
        {
            if (!target.HasTarget) return;

            var desiredVelocity = Vector3.zero;
            var lookDirection = target.TargetDirection;

            if (target.TargetDistance > _rangeStat.value)
            {
                desiredVelocity = target.TargetDirection * _moveStat.value;
            }
            else if (target.TargetDistance < _rangeStat.value)
            {
                desiredVelocity = -target.TargetDirection * _moveStat.value;
            }

            desiredVelocity += avoidance.avoidanceDirection;
            
            desiredVelocity = Vector3.Lerp(_lastVelocity, desiredVelocity, Time.deltaTime * acceleration);
            
            _lastVelocity = desiredVelocity;

            movement.SetLookDirection(lookDirection);
            movement.SetVelocity(desiredVelocity);
        }
    }
}