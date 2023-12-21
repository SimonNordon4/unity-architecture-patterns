using UnityEngine;

namespace Classic.Actors
{
    [RequireComponent(typeof(ActorHealth))]
    public class ActorDieOnReset : ActorComponent
    {
        private ActorHealth _actorHealth;

        private void Awake()
        {
            _actorHealth = GetComponent<ActorHealth>();
        }

        public override void Reset()
        {
            _actorHealth.SetHealth(0);
        }
    }
}