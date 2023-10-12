using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterTarget : MonoBehaviour
    {
        [SerializeField] private Stats stats;
        [SerializeField] private float updateInterval = 0.05f;
        private float _timeSinceLastUpdate;
        [field:SerializeField] public LayerMask targetLayer { get; private set; }
        private readonly Collider[] _targets = new Collider[64];

        public Transform closestTransform { get; private set; } = null;
        public float distance { get; private set; } = Mathf.Infinity;
        public Vector3 targetDirection { get; private set; } = Vector3.zero;

        public bool hasTarget;

        public Transform GetClosestTarget()
        {
            targetDirection = Vector3.zero;
            
            // TODO: This needs to be maximum range of all weapons, eventually.
            var radius = stats.range.value;
            var hits = Physics.OverlapSphereNonAlloc(transform.position, radius, _targets, targetLayer);

            if (hits <= 0) return null;
            
            var closestDistance = Mathf.Infinity;
            closestTransform = null;
            
            for(var i =0; i< hits; i++)
            {
                var target = _targets[i].transform;
                var dist = Vector3.Distance(transform.position, target.position);

                dist -= target.localScale.x * 0.5f;
                if (!(dist < closestDistance)) continue;
                
                closestDistance = dist;
                distance = closestDistance;
                closestTransform = target;
                targetDirection = Vector3.ProjectOnPlane((target.position - transform.position).normalized, Vector3.up);
                hasTarget = true;
            }
            
            return closestTransform;
        }

        private void Update()
        {
            if (hasTarget)
            {
                if(Vector3.Distance(transform.position, closestTransform.position) > stats.range.value)
                {
                    Reset();
                }
            }

            
            if(_timeSinceLastUpdate < updateInterval)
            {
                _timeSinceLastUpdate += Time.deltaTime;
                return;
            }
            
            GetClosestTarget();
        }

        public void Reset()
        {
            closestTransform = null;
            distance = Mathf.Infinity;
            hasTarget = false;
        }
    }
}