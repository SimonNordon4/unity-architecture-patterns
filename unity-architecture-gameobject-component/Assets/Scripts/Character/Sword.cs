using GameObjectComponent.Game;
using GameplayComponents.Combat;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameObjectComponent.Character
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private Stats stats;
        [SerializeField] private CombatTarget target;
        private Stat meleeDamage => stats.GetStat(StatType.MeleeDamage);
        private Stat meleeKnockBack => stats.GetStat(StatType.MeleeKnockBack);

        private void OnTriggerEnter(Collider other)
        {
            // check if other is on character target layer
            if (target.targetLayer != (target.targetLayer | (1 << other.gameObject.layer))) return;
            
            if(TryGetComponent<DamageReceiver>(out var damageReceiver))
                damageReceiver.TakeDamage(Mathf.RoundToInt(meleeDamage.value));

            if (TryGetComponent<KnockBackReceiver>(out var knockBackReceiver))
            {
                // direction is equal to the direction from the enemy to the player.
                var direction = parent.transform.position - other.transform.position;
                direction = Vector3.ProjectOnPlane(-direction, Vector3.up).normalized;
                knockBackReceiver.ApplyKnockBack(direction * meleeKnockBack.value);
            }
        }
    }
}