using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class HealthPack : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private int baseHealthRecovery = 3;
        [SerializeField] private float missingHealthRecoveryMultiplier = 0.1f;

        public UnityEvent onPickedUp = new();

        private void OnTriggerEnter(Collider other)
        {
            // check if other is in target layer
            if (targetLayer != (targetLayer | (1 << other.gameObject.layer))) return;
            
            // check if there's a health component
            if (!other.TryGetComponent<Health>(out var health)) return;
            
            var missingHealth = health.maxHealth - health.currentHealth;
            var healthToRecover = baseHealthRecovery + (int)(missingHealth * missingHealthRecoveryMultiplier);
            health.AddHealth(healthToRecover);
            onPickedUp.Invoke();
            Destroy(gameObject);
        }
    }
}