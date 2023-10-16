using System;
using UnityEngine;

namespace Classic.Actor
{
    public class DamageReceiver : ActorComponent
    {
        public event Action<int> OnDamageTaken;

        public void TakeDamage(int damage)
        {
            OnDamageTaken?.Invoke(damage);
        }
    }
}