using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class CombatTarget : MonoBehaviour
    {
        [SerializeField] private Transform initialTarget;
        [field: SerializeField] public LayerMask targetLayer { get; private set; }
        public bool HasTarget { get; private set; }
        public Transform Target { get; private set; }

        public Vector3 TargetDirection => HasTarget ? (Target.position - _transform.position).normalized : -Vector3.forward;
        public float TargetDistance => HasTarget ? Vector3.Distance(_transform.position, Target.position) : 0f;

        private readonly Collider[] _colliders = new Collider[128];
        private Transform _transform;
        private GameObjectEvents _targetEvents;

        private void Awake()
        {
            _transform = transform;
            if (initialTarget != null)
            {
                SetTarget(initialTarget);
            }
        }
        
        public void SetTarget(Transform newTarget)
        {
            if (targetLayer != (targetLayer | (1 << newTarget.gameObject.layer)))
            {
                Debug.LogWarning("Target is not on the target layer.", this);
            }
            
            Target = newTarget;
            HasTarget = true;
            _targetEvents = GetGameObjectEvents(Target);
            _targetEvents.OnDisabled += RemoveTarget;
        }
        
        public void ClearTarget()
        {
            if (_targetEvents != null)
            {
                _targetEvents.OnDisabled -= RemoveTarget;
                _targetEvents = null;
            }
            HasTarget = false;
            Target = null;
        }

        public void GetClosestTarget(float range)
        {
            if (_targetEvents != null)
            {
                _targetEvents.OnDisabled -= RemoveTarget;
            }
            
            var count = Physics.OverlapSphereNonAlloc(transform.position, range, _colliders, targetLayer);
            
            if (count == 0)
            {
                HasTarget = false;
                return;
            }
            
            var closestDistance = float.MaxValue;
            var closestTarget = _colliders[0].transform;
            
            for (var i = 0; i < count; i++)
            {
                var currentCollider = _colliders[i].transform;
                var direction = currentCollider.position - _transform.position;
                var distance = direction.magnitude;

                if (distance > closestDistance) continue;
                
                closestDistance = distance;
                closestTarget = currentCollider;
            }
            
            HasTarget = true;
            Target = closestTarget;
            
            _targetEvents = GetGameObjectEvents(Target);
            _targetEvents.OnDisabled += RemoveTarget;
        }

        private GameObjectEvents GetGameObjectEvents(Transform sampleTarget)
        {
            return sampleTarget.TryGetComponent<GameObjectEvents>(out var events) ? events : Target.gameObject.AddComponent<GameObjectEvents>();
        }

        public void RemoveTarget()
        {
            if (_targetEvents != null)
            {
                _targetEvents.OnDisabled -= RemoveTarget;
                _targetEvents = null;
            }
            HasTarget = false;
            Target = null;
        }
    }
}