using System;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(DamageReceiver))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Dodge))]
    [RequireComponent(typeof(Armor))]
    public class DamageHandler : MonoBehaviour
    {
        private DamageReceiver _damageReceiver;
        private Health _health;
        private Dodge _dodge;
        private Armor _block;

        public event Action<Vector3> OnDodge;
        public event Action<Vector3, int> OnCriticalHit;
        public event Action<Vector3, int> OnDamage;

        private void Awake()
        {
            _damageReceiver = GetComponent<DamageReceiver>();
            _health = GetComponent<Health>();
            _dodge = GetComponent<Dodge>();
            _block = GetComponent<Armor>();
        }

        private void OnEnable()
        {
            _damageReceiver.OnDamageReceived += OnDamageReceived;
        }

        private void OnDisable()
        {
            _damageReceiver.OnDamageReceived -= OnDamageReceived;
        }

        private void OnDamageReceived(int damageAmount, bool isCritical=false)
        {
            if (_dodge.CalculateDodge())
            {
                OnDodge?.Invoke(transform.position);
                return;
            }

            damageAmount = _block.CalculateArmorReduction(damageAmount);
            OnDamage?.Invoke(transform.position, damageAmount);
            _health.TakeDamage(damageAmount);
        }
    }
}