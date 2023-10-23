using System;
using Classic.Actors;
using Classic.Pools;
using UnityEngine;

namespace Classic.Enemies
{
    [RequireComponent(typeof(ActorHealth))]
    [RequireComponent(typeof(ParticlePool))]
    public class Enemy : MonoBehaviour
    {
        private ActorHealth _health;
        private ParticlePool _deathParticlePool;

        private void Awake()
        {
            _health = GetComponent<ActorHealth>();
            _deathParticlePool = GetComponent<ParticlePool>();
        }

        private void Start()
        {
            _health.OnDeath += OnDeath;
        }
        
        private void OnDeath()
        {
            _deathParticlePool.GetForParticleDuration(transform.position);
            Destroy(gameObject);
        }
    }
}