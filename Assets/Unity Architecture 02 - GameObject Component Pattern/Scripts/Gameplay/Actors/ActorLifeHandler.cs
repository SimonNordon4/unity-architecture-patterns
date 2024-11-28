using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{

    [RequireComponent(typeof(Movement))]
    public class ActorLifeHandler : MonoBehaviour
    {
        
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticleSystem deathParticle;
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private TrailRenderer trailRenderer;
        private Movement _movement;
        private LayerMask _originalLayerMask;
        private LayerMask _neutralLayerMask = 0;
        
        public UnityEvent onSpawned = new();

        private void Awake()
        {
            _originalLayerMask = gameObject.layer;
        }

        void OnEnable()
        {
            _movement.enabled = false;
            spawnInParticle.Play();
            enemyMesh.SetActive(false);
            StopAllCoroutines();
            gameObject.layer = _neutralLayerMask;
            StartCoroutine(SpawnIn());
            trailRenderer.Clear();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator SpawnIn()
        {
            var countdown = spawnTime;
            while (countdown > 0)
            {
                countdown -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            spawnInParticle.Stop();
            deathParticle.Play();
            _movement.enabled = true;
            gameObject.layer = _originalLayerMask;
            onSpawned.Invoke();
            enemyMesh.SetActive(true);
        }
    }
}