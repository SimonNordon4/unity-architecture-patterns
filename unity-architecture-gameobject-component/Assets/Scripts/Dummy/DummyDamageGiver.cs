using GameObjectComponent.GameplayComponents.Combat;
using GameObjectComponent.GameplayComponents.Life;
using UnityEngine;

namespace GameObjectComponent.Dummy
{
    public class DummyDamageGiver : MonoBehaviour
    {
        [SerializeField] private int damageAmount;
        [SerializeField] private LayerMask damageMask;

        private void OnTriggerEnter(Collider other)
        {
            if( damageMask != (damageMask | (1 << other.gameObject.layer))) return;
            
            if(other.TryGetComponent<DamageReceiver>(out var damageReceiver))
                damageReceiver.TakeDamage(damageAmount);
        }
    }
}