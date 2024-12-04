using System.Collections;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ChargeWeapon : BaseWeapon
    {
        [SerializeField]private float chargeSpeed = 10f;
        [SerializeField]private float chargeDistance = 5f;
        private bool _isCharging = false;

        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if (_isCharging) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(ChargeAtTarget(direction, target, info));
        }

        private IEnumerator ChargeAtTarget(Vector3 direction, CombatTarget target, WeaponStatsInfo info)
        {
            _isCharging = true;
            float chargedDistance = 0f;

            while (chargedDistance < chargeDistance)
            {
                float distanceThisFrame = chargeSpeed * Time.deltaTime;
                transform.position += direction * distanceThisFrame;
                chargedDistance += distanceThisFrame;

                // Check for collision with target
                if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
                {
                    ApplyDamage(target, info);
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            _isCharging = false;
        }

        private void ApplyDamage(CombatTarget target, WeaponStatsInfo info)
        {
            // Assuming CombatTarget has a method to take damage
            if(target.TryGetComponent<DamageReceiver>(out var damageReceiver))
            {
                damageReceiver.TakeDamage(info.Damage);
            }
        }
    }
}