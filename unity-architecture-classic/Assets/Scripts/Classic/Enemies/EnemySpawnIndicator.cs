using System;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemySpawnIndicator : MonoBehaviour
    {
        public event Action OnCompleted;
        public event Action OnCancelled;
        
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private LayerMask characterLayer;
        private float _elapsedTime = 0.0f;

        private void Update()
        {
            _elapsedTime += GameTime.deltaTime;
            if (_elapsedTime < spawnTime) return;
            OnCompleted?.Invoke();
            Destroy(gameObject);            
        }

        private void OnTriggerEnter(Collider other)
        {
            if(characterLayer != (characterLayer | (1 << other.gameObject.layer))) return;
            
            OnCancelled?.Invoke();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            OnCompleted = null;
            OnCancelled = null;
        }
    }
}