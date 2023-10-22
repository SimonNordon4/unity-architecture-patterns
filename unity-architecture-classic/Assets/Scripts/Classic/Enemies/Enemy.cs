using System;
using UnityEngine;

namespace Classic.Enemies
{
    public class Enemy : MonoBehaviour
    {
        public event Action OnDeath;

        public void Die()
        {
            OnDeath?.Invoke();
        }

        public void OnDisable()
        {
            OnDeath = null;
        }
    }
}