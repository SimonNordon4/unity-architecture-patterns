using Classic.Actors;
using Classic.Pools;
using UnityEngine;

namespace Classic.Enemies
{
    [RequireComponent(typeof(ActorHealth))]
    [RequireComponent(typeof(ParticlePool))]
    public class EnemyDeathHandler : ActorComponent
    {
        private Enemy _enemy;
        [SerializeField] private EnemyDefinition _definition;
        private ActorHealth _health;
        private ParticlePool _deathParticlePool;
        [SerializeField] private EnemyPool pool;

        public void Construct(EnemyPool newPool)
        {
            pool = newPool;
        }
        
        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
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
            pool.Return(_enemy,_definition);
        }
    }
}