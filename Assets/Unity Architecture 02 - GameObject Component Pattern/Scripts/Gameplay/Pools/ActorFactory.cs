using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private Level level;
        [field:SerializeField] public Transform initialTarget { get; private set; }
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private PoolableActor actorPrefab;

        public PoolableActor Create(Vector3 position = new(), bool startActive = true)
        {
            actorPrefab.gameObject.SetActive(false);
            
            var actor = Instantiate(actorPrefab, position, Quaternion.identity, null);

            if (actor.TryGetComponent<Movement>(out var movement))
                movement.Construct(level);

            if (actor.TryGetComponent<CombatTarget>(out var target))
                target.SetTarget(initialTarget);

            // if (actor.TryGetComponent<ParticlePool>(out var particlePool))
            //     particlePool.Construct(deathParticlePool);
            
            // if(actor.TryGetComponent<ActorSpawnDelay>(out var spawnDelay))
            //     spawnDelay.Construct(deathParticlePool);
            
            if(actor.TryGetComponent<ProjectilePool>(out var bulletPool))
                bulletPool.Construct(projectilePool);
            
            // if(actor.TryGetComponent<SoundProxy>(out var actorSoundProxy))
            //     actorSoundProxy.Construct(soundManager);
            
            actor.gameObject.SetActive(true);

            return actor;
        }
    }
}

