using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class AvoidAllies : MonoBehaviour
    {
        [SerializeField] private float repulsionForce = 1f;
        private CapsuleCollider _body;
        public Vector3 avoidanceDirection { get; private set; }
        private readonly List<Transform> _nearbyEnemies = new(4);
        private float _radius;

        private void Awake()
        {
            _body = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            _radius = _body.radius;
        }

        private void Update()
        {
            var totalEnemies = _nearbyEnemies.Count;
            avoidanceDirection = Vector3.zero;
            for (var i = 0; i < totalEnemies; i++)
            {
                var enemy = _nearbyEnemies[i];
                if (enemy == null)
                {
                    _nearbyEnemies.RemoveAt(i);
                    i--; // Adjust the index to account for the removed enemy
                    totalEnemies--; // Adjust the total count of enemies
                    continue;
                }

                var toEnemy = enemy.position - transform.position;

                // Calculate distance and project direction on plane
                var distance = toEnemy.magnitude;
                var enemyDir = Vector3.ProjectOnPlane(toEnemy, Vector3.up).normalized;

                // Scale the direction based on distance (closer enemies have stronger influence)
                var enemyRadius = enemy.localScale.x;
                var touchDistance = _radius + enemyRadius;
                if (distance < touchDistance) // We don't want to divide by zero or a negative number
                {
                    // normalize the distance.
                    var normalizedDistance = distance / touchDistance;
                    // We inverse the force so it's weak when far away, and very strong when close.
                    var force = 1 - normalizedDistance;
                    // make it REALLY weak when far away.
                    var polynomialForce = force * force * force;
                    avoidanceDirection -= enemyDir * (polynomialForce * repulsionForce);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(gameObject.layer == other.gameObject.layer && _nearbyEnemies.Count < 4)
                _nearbyEnemies.Add(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if(gameObject.layer == other.gameObject.layer && _nearbyEnemies.Contains(other.transform))
                _nearbyEnemies.Remove(other.transform);
        }
    }
}