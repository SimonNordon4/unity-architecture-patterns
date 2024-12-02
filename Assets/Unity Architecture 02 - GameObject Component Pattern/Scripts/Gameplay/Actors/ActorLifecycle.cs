using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ActorLifecycle : MonoBehaviour
    {
        [SerializeField] private float spawnInDelay = 1f;
        [SerializeField] private float spawnOutDelay = 1f;

        public UnityEvent OnSpawnIn = new UnityEvent();
        public UnityEvent OnSpawnOut = new UnityEvent();

        public void SpawnIn()
        {
            StartCoroutine(SpawnCoroutine(spawnInDelay, OnSpawnIn));
        }

        public void SpawnOut()
        {
            StartCoroutine(SpawnCoroutine(spawnOutDelay, OnSpawnOut));
        }

        private IEnumerator SpawnCoroutine(float delay, UnityEvent spawnEvent)
        {
            yield return new WaitForSeconds(delay);
            spawnEvent.Invoke();
        }
    }
}