using System.Collections;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(MoveBase))]
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(KnockBack))]
    public class Charge : MonoBehaviour
    {
        private Movement _movement;
        private MoveBase _moveBase;
        private CombatTarget _combatTarget;
        private Stat _damageStat;
        private KnockBack _knockBack;

        [Header("Charge Values")]
        [SerializeField] private float chargeSpeed = 10f;
        [SerializeField] private float chargeDistance = 3f;
        [SerializeField] private float chargeCooldown = 2f;
        [SerializeField] private float chargeUpTime = 0.5f;

        private float _timeSinceLastCharge;
        private bool _isCharging = false;
        private Vector3 _lastDir;

        private void Start()
        {
            _moveBase = GetComponent<MoveBase>();
            _combatTarget = GetComponent<CombatTarget>();
            _movement = GetComponent<Movement>();
            var stats = GetComponent<Stats>();
            _damageStat = stats.GetStat(StatType.Damage);
            _knockBack = GetComponent<KnockBack>();

            _timeSinceLastCharge = chargeCooldown;
        }

        private void Update()
        {
            if (_isCharging) return;

            _timeSinceLastCharge += Time.deltaTime;

            if (_combatTarget.TargetDistance <= chargeDistance * 0.5f && _timeSinceLastCharge >= chargeCooldown)
            {
                StartCoroutine(ChargeAtTarget(_combatTarget.TargetDirection));
                _timeSinceLastCharge = 0f;
            }
        }

        private IEnumerator ChargeAtTarget(Vector3 dir)
        {
            _isCharging = true;
            _moveBase.enabled = false;
            _knockBack.enabled = false;
            _movement.SetVelocity(Vector3.zero);

            yield return new WaitForSeconds(chargeUpTime);

            // Apply Velocity for Charging
            _movement.SetVelocity(dir * chargeSpeed);

            float chargedDistance = 0f;

            while (chargedDistance < chargeDistance)
            {
                // Calculate distance moved this frame
                float distanceThisFrame = chargeSpeed * Time.deltaTime;
                chargedDistance += distanceThisFrame;

                // Check distance to target
                float currentDistance = Vector3.Distance(transform.position, _combatTarget.Target.position);
                if (currentDistance <= 0.5f)
                {
                    ApplyDamageToTarget();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            // Stop Charging
            _movement.SetVelocity(Vector3.zero);
            _moveBase.enabled = true;
            _knockBack.enabled = true;
            _isCharging = false;
        }

        private void ApplyDamageToTarget()
        {
            if (_combatTarget.Target.TryGetComponent<DamageReceiver>(out var damageReceiver))
            {
                damageReceiver.TakeDamage(_damageStat.value);
            }
        }
    }
}