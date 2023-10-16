using UnityEngine;

namespace Classic.Actors
{
    public class ActorDamageHandler : ActorComponent
    {
        [SerializeField]private DamageReceiver damageReceiver;
        [SerializeField]private ActorHealth health;
        [SerializeField]private ActorDodge dodge;
        private bool _hasDodgeReference = false;
        [SerializeField]private ActorBlock block;
        private bool _hasBlockReference = false;

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
                if (dodge.CalculateDodge()) return;
            }

            if (_hasBlockReference)
            {
                damageAmount = block.CalculateBlock(damageAmount);
            }
            
            health.TakeDamage(damageAmount);
        }
    }
}