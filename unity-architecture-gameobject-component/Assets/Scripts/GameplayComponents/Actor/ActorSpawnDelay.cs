using System.Collections;
using GameObjectComponent.Definitions;
using GameObjectComponent.Pools;
using UnityEngine;

namespace GameplayComponents.Actor
{
    public class ActorSpawnDelay : MonoBehaviour
    {
        [SerializeField] private ActorDefinition definition;
        [SerializeField] private LayerMask interruptLayer;
        [SerializeField] private ActorState actorState;
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticlePool deathParticlePool;
        [SerializeField] private float spawnTime = 1f;
        
        private bool _isSpawned = false;
        
        public void Construct(ParticlePool newDeathParticlePool)
        {
            deathParticlePool = newDeathParticlePool;
        }

        private void Start()
        {
            var main = spawnInParticle.main; 
            main.startColor = definition.enemyColor;
        }

        void OnEnable()
        {
            actorState.DisableActorComponents();
            spawnInParticle.Play();
            enemyMesh.SetActive(false);
            StopAllCoroutines();
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
            yield return new WaitForSeconds(spawnTime);
            spawnInParticle.Stop();
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
            actorState.EnableActorComponents();
            _isSpawned = true;
            enemyMesh.SetActive(true);
        }

        private void CancelSpawnIn()
        {
            StopAllCoroutines();
            deathParticlePool.GetForParticleDuration(transform.position, definition.enemyColor);
        }
    }
}