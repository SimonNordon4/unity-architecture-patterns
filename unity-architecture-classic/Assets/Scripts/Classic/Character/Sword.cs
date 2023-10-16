using Classic.Actors;
using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private Stats stats;
        [SerializeField] private CharacterTarget target;

        private void OnTriggerEnter(Collider other)
        {
            // check if other is on character target layer
            if (target.targetLayer != (target.targetLayer | (1 << other.gameObject.layer))) return;
            
            if(TryGetComponent<DamageReceiver>(out var damageReceiver))
                damageReceiver.TakeDamage(Mathf.RoundToInt(stats.meleeDamage.value));

            if (TryGetComponent<KnockBackReceiver>(out var knockBackReceiver))
            {
                // direction is equal to the direction from the enemy to the player.
                var direction = parent.transform.position - other.transform.position;
                direction = Vector3.ProjectOnPlane(-direction, Vector3.up).normalized;
                knockBackReceiver.ApplyKnockBack(direction * stats.meleeKnockBack.value);
            }
        }
    }
}