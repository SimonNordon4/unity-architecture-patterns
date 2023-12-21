using Classic.Actors;
using UnityEngine;

namespace Classic.Dummy
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