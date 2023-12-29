using UnityEngine;

namespace Classic.Actors.Weapons
{
    [RequireComponent(typeof(ActorTarget))]
    public class BodySlamWeapon : BaseWeapon
    {

        private ActorTarget _targetComponent;

        private void Start()
        {
            _targetComponent = GetComponent<ActorTarget>();
        }

        public override void Attack(MeleeStatsInfo info)
        {
            if(!_targetComponent.target.TryGetComponent<DamageReceiver>(out var receiver)) return;
            receiver.TakeDamage(info.Damage);
        }
    }
}