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

        private void Awake()
        {
            damageReceiver = GetComponent<DamageReceiver>();
            health = GetComponent<Health>();
        }

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
            if (_hasDodgeReference)
            {
                if (dodge.CalculateDodge())
                {
                    return;
                }
            }

            if (_hasBlockReference)
            {
                damageAmount = block.CalculateBlock(damageAmount);
                if (damageAmount <= 0)
                {
                    return;
                }
            }
            
            health.TakeDamage(damageAmount);
        }
    }
}