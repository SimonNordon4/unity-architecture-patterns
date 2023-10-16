using Classic.Actors;
using UnityEngine;

namespace Classic.Debugging

{
    public class ActorDamageDebugger : DebugComponent
    {
        [SerializeField] private DamageReceiver damageReceiver;
        [SerializeField] private ActorHealth health;
        [SerializeField] private ActorBlock block;
        [SerializeField] private ActorDodge dodge;
        [SerializeField] private ActorRevive revive;

        private void OnEnable()
        {
            if (damageReceiver != null)
            {
                damageReceiver.OnDamageReceived += i => Print($"{damageReceiver.gameObject.name} took {i} damage");
            }
            
            if (health != null)
            {
                health.OnHealthChanged += (x => Print($"{health.gameObject.name} Health: {x}"));
                health.OnDeath += () => Print($"{health.gameObject.name} Died");
            }

            if (block != null)
            {
                block.onFullBlock.AddListener(() => Print($"{block.gameObject.name} Blocked"));
            }

            if (dodge != null)
            {
                dodge.onDodged.AddListener(() => Print($"{dodge.gameObject.name} Dodged"));    
            }

            if (revive != null)
            {
                revive.onRevived.AddListener(() => Print($"{revive.gameObject.name} Revived"));
            }
            
        }

    }
}