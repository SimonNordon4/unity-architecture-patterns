using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class EntitySpawnDelay : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private MeshRenderer enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticlePool deathParticlePool;
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private TrailRenderer trailRenderer;
        
        private bool _isSpawned = false;

        public UnityEvent onCancelled = new();
        public UnityEvent onSpawned = new();
        
        public void Construct(ParticlePool newDeathParticlePool)
        {
            deathParticlePool = newDeathParticlePool;
        }

        private void Awake()
        {
            _originalLayer = gameObject.layer;
        }

        private void Start()
        {
            var main = spawnInParticle.main; 
            main.startColor = definition.enemyColor;
        }

        void OnEnable()
        {
            movement.canMove = false;
            spawnInParticle.Play();
            enemyMesh.SetActive(false);
            StopAllCoroutines();
            gameObject.layer = spawningLayer;
            StartCoroutine(SpawnIn());
            trailRenderer.Clear();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _isSpawned = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_isSpawned) return;
            if (interruptLayer == (interruptLayer | (1 << other.gameObject.layer)))
            {
                CancelSpawnIn();
            }
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
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
            movement.canMove = true;
            _isSpawned = true;
            gameObject.layer = _originalLayer;
            onSpawned.Invoke();
            enemyMesh.SetActive(true);
        }

        private void CancelSpawnIn()
        {
            gameObject.layer = _originalLayer;
            onCancelled.Invoke();
            StopAllCoroutines();
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
        }

        public void PlayDeathParticle()
        {
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
        }
    }
}