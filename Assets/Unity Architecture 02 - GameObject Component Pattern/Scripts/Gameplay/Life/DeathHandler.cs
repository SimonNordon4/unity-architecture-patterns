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

        [SerializeField]private float deathDelay = 1f;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthDepleted.AddListener(OnHealthDepleted);
        }

        private void OnDisable()
        {
            _health.OnHealthDepleted.RemoveListener(OnHealthDepleted);
        }

        private void OnHealthDepleted()
        {
            StartCoroutine(OnDeathCoroutine());
        }

        private IEnumerator OnDeathCoroutine()
        {
            yield return new WaitForSeconds(deathDelay);
            onDeath?.Invoke();
        }
    }
}