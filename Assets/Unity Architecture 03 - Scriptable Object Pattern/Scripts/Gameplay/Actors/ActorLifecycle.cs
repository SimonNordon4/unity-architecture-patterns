using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ActorLifecycle : MonoBehaviour
    {
        private float spawnInDelay = 1f;
        private float spawnOutDelay = 0.3f;

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