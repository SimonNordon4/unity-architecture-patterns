using GameObjectComponent.Game;
using GameObjectComponent.General;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class CombatTarget : GameplayComponent
    {
        [SerializeField] private Transform initialTarget;
        [field: SerializeField] public LayerMask targetLayer { get; private set; }
        public bool hasTarget { get; private set; }
        public Transform target { get; private set; }

        public Vector3 targetDirection => hasTarget ? (target.position - _transform.position).normalized : Vector3.zero;
        public float targetDistance => hasTarget ? Vector3.Distance(_transform.position, target.position) : 0f;

        private readonly Collider[] _colliders = new Collider[128];
        private Transform _transform;
        private GameObjectEvents _targetEvents;
        private float _maximumRange;

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
            
            target = newTarget;
            hasTarget = true;
            _targetEvents = GetGameObjectEvents(target);
            _targetEvents.OnDisabled += RemoveTarget;
        }
        
        public void ClearTarget()
        {
            if (_targetEvents != null)
            {
                _targetEvents.OnDisabled -= RemoveTarget;
                _targetEvents = null;
            }
            hasTarget = false;
            target = null;
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
                hasTarget = false;
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
            
            hasTarget = true;
            target = closestTarget;
            
            _targetEvents = GetGameObjectEvents(target);
            _targetEvents.OnDisabled += RemoveTarget;
        }

        private GameObjectEvents GetGameObjectEvents(Transform sampleTarget)
        {
            return sampleTarget.TryGetComponent<GameObjectEvents>(out var events) ? events : target.gameObject.AddComponent<GameObjectEvents>();
        }

        public void RemoveTarget()
        {
            if (_targetEvents != null)
            {
                _targetEvents.OnDisabled -= RemoveTarget;
                _targetEvents = null;
            }
            hasTarget = false;
            target = null;
        }
    }
}