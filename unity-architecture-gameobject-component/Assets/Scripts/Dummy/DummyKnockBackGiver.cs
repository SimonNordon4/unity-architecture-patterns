using GameplayComponents.Combat;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameObjectComponent.Dummy
{
    public class DummyKnockBackGiver : MonoBehaviour
    {
        [SerializeField] private float knockBackAmount = 5f;
        [SerializeField] private LayerMask knockBackMask;

        private void OnTriggerEnter(Collider other)
        {
            if( knockBackMask != (knockBackMask | (1 << other.gameObject.layer))) return;
            if (!other.TryGetComponent<KnockBackReceiver>(out var knockBackReceiver)) return;
            
            var direction = (other.transform.position - transform.position).normalized;
            knockBackReceiver.ApplyKnockBack(knockBackAmount * direction);
        }
    }
}