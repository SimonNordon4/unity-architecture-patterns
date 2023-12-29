using System;
using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Life
{
    [RequireComponent(typeof(DamageReceiver))]
    [RequireComponent(typeof(Health))]
    public class DamageHandler : GameplayComponent
    {
        private DamageReceiver _damageReceiver;
        private Health _health;
        
        public event Action<GameObject,int> OnDamageTaken;
        public event Action<GameObject> OnDodge;
        public event Action<GameObject> OnBlock;
        
        [SerializeField]private Dodge dodge;
        private bool _hasDodgeReference = false;
        [SerializeField]private Block block;
        private bool _hasBlockReference = false;

        private void Awake()
        {
            _damageReceiver = GetComponent<DamageReceiver>();
            _health = GetComponent<Health>();
        }

        private void Start()
        {
            _hasBlockReference = block != null;
            _hasDodgeReference = dodge != null;
        }

        private void OnEnable()
        {
            _damageReceiver.OnDamageReceived += OnDamageReceived;
        }

        private void OnDisable()
        {
            _damageReceiver.OnDamageReceived -= OnDamageReceived;
        }
        
        private void OnDamageReceived(int damageAmount)
        {
            if (_hasDodgeReference)
            {
                if (dodge.CalculateDodge())
                {
                    OnDodge?.Invoke(gameObject);
                    return;
                }
            }

            if (_hasBlockReference)
            {
                damageAmount = block.CalculateBlock(damageAmount);
                if (damageAmount <= 0)
                {
                    OnBlock?.Invoke(gameObject);
                    return;
                }
            }
            
            _health.TakeDamage(damageAmount);
            OnDamageTaken?.Invoke(gameObject, damageAmount);
        }
    }
}