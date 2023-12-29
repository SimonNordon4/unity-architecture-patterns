using System;
using UnityEngine;

namespace Classic.Actors
{
    public class DamageReceiver : ActorComponent
    {
        public event Action<int> OnDamageReceived;

        public void TakeDamage(int damage)
        {
            OnDamageReceived?.Invoke(damage);
        }
    }
}