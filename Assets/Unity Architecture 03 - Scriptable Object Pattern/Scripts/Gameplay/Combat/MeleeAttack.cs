
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(CombatTarget))]
    public class MeleeAttack : MonoBehaviour
    {
        [SerializeField] private BaseWeapon weapon;
        private CombatTarget _target;
        private Stat _damage;
        private Stat _knockBack;
        private Stat _range;
        private Stat _fireRate;
        private Stat _critChance;

        private float _timeSinceLastAttack = 0f;

        private void Awake()
        {
            var stats = GetComponent<Stats>();
            _damage = stats.GetStat(StatType.Damage);
            _knockBack = stats.GetStat(StatType.KnockBack);
            _range = stats.GetStat(StatType.Range);
            _fireRate = stats.GetStat(StatType.FireRate);
            _target = GetComponent<CombatTarget>();
            _critChance = stats.GetStat(StatType.CritChance);
        }

        private void Update()
        {
            var inverseAttackSpeed = 10f / _fireRate.value;
            if (_timeSinceLastAttack < inverseAttackSpeed)
            {
                _timeSinceLastAttack += Time.deltaTime;
                return;
            }

            if (!_target.HasTarget) return;

            var distance = _target.TargetDistance;

            if (_target.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
            {
                distance -= capsuleCollider.radius;
            }

            if (distance > _range.value) return;

            var info = new WeaponStatsInfo
            {
                Damage = (int)_damage.value,
                KnockBack = _knockBack.value,
                Range = _range.value,
                AttackSpeed = _fireRate.value,
                IsCrit = Random.Range(0f, 100f) < _critChance.value
            };

            weapon.Attack(info, _target);

            _timeSinceLastAttack = 0f;
        }
    }
}