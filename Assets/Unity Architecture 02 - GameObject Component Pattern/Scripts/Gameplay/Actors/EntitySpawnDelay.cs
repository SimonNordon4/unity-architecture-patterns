using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class EntitySpawnDelay : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticleSystem deathParticle;
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private TrailRenderer trailRenderer;

        private LayerMask _originalLayerMask;
        private LayerMask _neutralLayerMask = 0;
        
        private bool _isSpawned = false;

        public UnityEvent onSpawned = new();

        private void Awake()
        {
            _originalLayerMask = gameObject.layer;
        }

        void OnEnable()
        {
            movement.enabled = false;
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
            _isSpawned = false;
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
            movement.enabled = true;
            _isSpawned = true;
            gameObject.layer = _originalLayerMask;
            onSpawned.Invoke();
            enemyMesh.SetActive(true);
        }
    }
}