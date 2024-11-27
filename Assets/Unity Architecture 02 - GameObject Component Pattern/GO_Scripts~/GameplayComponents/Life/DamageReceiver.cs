using System;

namespace GameplayComponents.Life
{
    public class DamageReceiver : GameplayComponent
    {
        public event Action<int> OnDamageReceived;

        public void TakeDamage(int damage)
        {
            OnDamageReceived?.Invoke(damage);
        }
    }
}