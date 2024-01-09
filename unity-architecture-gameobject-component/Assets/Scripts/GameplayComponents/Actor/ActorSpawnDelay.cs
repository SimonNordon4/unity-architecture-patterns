using System.Collections;
using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using GameObjectComponent.Pools;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Actor
{
    public class ActorSpawnDelay : MonoBehaviour
    {
        [SerializeField] private ActorDefinition definition;
        [SerializeField] private LayerMask interruptLayer;
        [SerializeField] private LayerMask spawningLayer;
        private LayerMask _originalLayer;
        [SerializeField] private GameplayStateController gameplayStateController;
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticlePool deathParticlePool;
        [SerializeField] private float spawnTime = 1f;
        
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
            gameplayStateController.DisableActorComponents();
            spawnInParticle.Play();
            enemyMesh.SetActive(false);
            StopAllCoroutines();
            gameObject.layer = spawningLayer;
            StartCoroutine(SpawnIn());
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
                countdown -= GameTime.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            spawnInParticle.Stop();
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
            gameplayStateController.EnableActorComponents();
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