using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyMoveTowardsTarget : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        [SerializeField] private EnemyStats stats;
        [SerializeField] private LayerMask targetMask;
        private Transform _transform;
        private Transform _target;
        private Collider[] _colliders = new Collider[1];

        private void OnEnable()
        {
            _transform = transform;
            _target = GetAnyTarget();

            if (_target == null)
            {
                Debug.LogError("no target found");
            }
        }

        public Transform GetAnyTarget()
        {
            var hits = Physics.OverlapSphereNonAlloc(transform.position,
                Mathf.Max(scope.level.bounds.x,scope.level.bounds.y),
                _colliders,
                targetMask);
            return hits == 0 ? null : _colliders[0].transform;
        }

        private void Update()
        {
            var position = _transform.position;
            var direction = (_target.position - position).normalized;
            stats.velocity = direction * stats.speed;
            position += stats.velocity * GameTime.deltaTime;
            _transform.position = position;
        }
    }
}