using GameObjectComponent.Game;
using GameObjectComponent.GameplayComponents.Actor;
using GameObjectComponent.GameplayComponents.Combat;
using GameObjectComponent.GameplayComponents.Life;
using GameObjectComponent.GameplayComponents.Locomotion;
using UnityEngine;

namespace GameObjectComponent.Character
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private ActorStats stats;
        [SerializeField] private CombatTarget target;

        private void OnTriggerEnter(Collider other)
        {
            // check if other is on character target layer
            if (target.targetLayer != (target.targetLayer | (1 << other.gameObject.layer))) return;
            
            if(TryGetComponent<DamageReceiver>(out var damageReceiver))
                damageReceiver.TakeDamage(Mathf.RoundToInt(stats.Map[StatType.MeleeDamage].value));

            if (TryGetComponent<KnockBackReceiver>(out var knockBackReceiver))
            {
                // direction is equal to the direction from the enemy to the player.
                var direction = parent.transform.position - other.transform.position;
                direction = Vector3.ProjectOnPlane(-direction, Vector3.up).normalized;
                knockBackReceiver.ApplyKnockBack(direction * stats.Map[StatType.MeleeKnockBack].value);
            }
        }
    }
}