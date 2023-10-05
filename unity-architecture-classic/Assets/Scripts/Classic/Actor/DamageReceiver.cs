using System;
using UnityEngine;

namespace Classic.Actor
{
    public class DamageReceiver : MonoBehaviour
    {
        public event Action<int> OnDamageTaken;

        public void TakeDamage(int damage)
        {
            OnDamageTaken?.Invoke(damage);
        }
    }
}