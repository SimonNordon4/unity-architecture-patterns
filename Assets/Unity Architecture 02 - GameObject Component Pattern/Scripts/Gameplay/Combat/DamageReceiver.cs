using System;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class DamageReceiver : MonoBehaviour
    {
        public event Action<int, bool> OnDamageReceived;

        public void TakeDamage(int damage, bool isCriticalHit = false)
        {
            OnDamageReceived?.Invoke(damage, isCriticalHit);
        }
    }
}