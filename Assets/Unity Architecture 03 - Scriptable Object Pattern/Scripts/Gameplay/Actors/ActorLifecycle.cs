using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ActorLifecycle : MonoBehaviour
    {
        [SerializeField]
        private float spawnInDelay = 1f;
        [SerializeField]
        private float spawnOutDelay = 0.3f;

        public UnityEvent OnCreated = new UnityEvent();
        public UnityEvent OnSpawnIn = new UnityEvent();
        public UnityEvent OnSpawnOut = new UnityEvent();

        public void OnEnable()
        {
            OnCreated.Invoke();
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