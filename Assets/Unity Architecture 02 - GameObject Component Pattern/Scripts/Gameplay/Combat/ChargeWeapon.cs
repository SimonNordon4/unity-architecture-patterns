using System.Collections;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Movement))]
    public class ChargeWeapon : BaseWeapon
    {
        private Movement _movement;
        [SerializeField] private float chargeSpeed = 10f;
        [SerializeField] private float chargeDistance = 5f;
        private bool _isCharging = false;

        private void Start()
        {
            _movement = GetComponent<Movement>();
        }

        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            if (_isCharging) return;

            Debug.Log("Starting Charge!!!!");
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(ChargeAtTarget(direction, target, info));
        }

        private IEnumerator ChargeAtTarget(Vector3 direction, CombatTarget target, WeaponStatsInfo info)
        {
            _movement.enabled = false;
            _isCharging = true;

            // Pause for 0.5 seconds before charging
            yield return new WaitForSeconds(0.5f);

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
            _movement.enabled = true;
        }

        private void ApplyDamage(CombatTarget target, WeaponStatsInfo info)
        {
            // Assuming CombatTarget has a method to take damage
            if (target.TryGetComponent<DamageReceiver>(out var damageReceiver))
            {
                damageReceiver.TakeDamage(info.Damage);
            }
        }
    }
}