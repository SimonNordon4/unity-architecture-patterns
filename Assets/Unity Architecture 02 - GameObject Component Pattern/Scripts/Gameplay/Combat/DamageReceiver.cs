using System;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class DamageReceiver : MonoBehaviour
    {
        public event Action<int> OnDamageReceived;

        public void TakeDamage(int damage)
        {
            OnDamageReceived?.Invoke(damage);
        }
    }
}