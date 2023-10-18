using Classic.Actors;
using UnityEngine;

namespace Classic.Dummy
{
    public class DummyKnockBackGiver : MonoBehaviour
    {
        [SerializeField] private int knockBackAmount;
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