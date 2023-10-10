using System.Collections;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Enemies.Enemy
{
    public class EnemySpawnController : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        [SerializeField] private GameObject enemyMesh;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float spawnInTime;
        [SerializeField] private ParticleSystem spawnInParticles;
        [SerializeField] private ParticleSystem onSpawnParticle;
        
        
        private float _timeSinceEnabled = 0.0f;
        private bool _hasSpawned = false;

        private void OnEnable()
        {
            spawnInParticles.Play();
            enemyMesh.SetActive(false);
            _hasSpawned = false;
        }

        private void Update()
        {
            if(_hasSpawned) return;
            
            _timeSinceEnabled += GameTime.deltaTime;
            if (!(_timeSinceEnabled > spawnInTime)) return;
            _hasSpawned = true;
            enemyMesh.SetActive(true);
            spawnInParticles.Stop();
            onSpawnParticle.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_hasSpawned) return;
            if (playerLayer != (playerLayer | 1 << other.gameObject.layer)) return;
            scope.Destroy();
        }
    }
}