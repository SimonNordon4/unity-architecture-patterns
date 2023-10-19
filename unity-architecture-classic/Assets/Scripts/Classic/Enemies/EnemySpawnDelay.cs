using System.Collections;
using Classic.Actors;
using Classic.Pools;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemySpawnDelay : MonoBehaviour
    {
        [SerializeField] private LayerMask interruptLayer;
        [SerializeField] private ActorState actorState;
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private ParticleSystem spawnInParticle;
        [SerializeField] private ParticlePool deathParticlePool;
        [SerializeField] private float spawnTime = 1f;
        private bool _isSpawned = false;
        
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
            deathParticlePool.GetForParticleDuration(transform.position);
            actorState.EnableActorComponents();
            _isSpawned = true;
            enemyMesh.SetActive(true);
        }

        private void CancelSpawnIn()
        {
            StopAllCoroutines();
            deathParticlePool.GetForParticleDuration(transform.position);
        }
    }
}