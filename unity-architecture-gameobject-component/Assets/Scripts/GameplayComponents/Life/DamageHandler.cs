using System;
using UnityEngine;

namespace GameplayComponents.Life
{
    public class DamageHandler : GameplayComponent
    {
        [SerializeField]
        private DamageReceiver damageReceiver;
        [SerializeField]
        private Health health;
        [SerializeField]private Dodge dodge;
        private bool _hasDodgeReference = false;
        [SerializeField]private Block block;
        private bool _hasBlockReference = false;
        
        [field:SerializeField] public bool isInvincible { get; set; } = false;
        
        public event Action<Vector3> OnBlock;
        public event Action<Vector3> OnDodge;
        public event Action<Vector3, int> OnDamage;

        private void Start()
        {
            _hasBlockReference = block != null;
            _hasDodgeReference = dodge != null;
        }

        private void OnEnable()
        {
            damageReceiver.OnDamageReceived += OnDamageReceived;
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived -= OnDamageReceived;
        }
        
        private void OnDamageReceived(int damageAmount)
        {
            if(isInvincible)
                return;
            
            if (_hasDodgeReference)
            {
                if (dodge.CalculateDodge())
                {
                    OnDodge?.Invoke(transform.position);
                    return;
                }
            }

            if (_hasBlockReference)
            {
                damageAmount = block.CalculateBlock(damageAmount);
                if (damageAmount <= 0)
                {
                    OnBlock?.Invoke(transform.position);
                    return;
                }
            }
            
            OnDamage?.Invoke(transform.position, damageAmount);
            health.TakeDamage(damageAmount);
        }
        
        public void MakeInvincible()
        {
            isInvincible = true;
        }
        
        public void MakeVulnerable()
        {
            isInvincible = false;
        }
    }
}