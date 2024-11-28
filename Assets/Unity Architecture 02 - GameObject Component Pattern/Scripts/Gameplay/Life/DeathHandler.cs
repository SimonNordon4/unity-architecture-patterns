using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Health))]
    public class DeathHandler : MonoBehaviour
    {
        public UnityEvent onDeath = new();
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthDepleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            _health.OnHealthDepleted -= OnHealthDepleted;
        }

        private void OnHealthDepleted()
        {
            onDeath?.Invoke();
        }
    }
}