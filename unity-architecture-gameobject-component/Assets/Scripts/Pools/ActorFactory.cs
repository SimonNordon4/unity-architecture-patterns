using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using GameObjectComponent.Pools;
using GameplayComponents.Actor;
using GameplayComponents.Combat;
using GameplayComponents.Locomotion;
using UnityEngine;
using GameObjectComponent.App;

namespace Pools
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [field:SerializeField] public Transform initialTarget { get; private set; }
        [SerializeField] private ParticlePool deathParticlePool;
        [SerializeField] private MunitionPool munitionPool;
        [SerializeField] private SoundManager soundManager;

        public PoolableActor Create(ActorDefinition actorDefinition, Vector3 position = new(), bool startActive = true)
        {
            actorDefinition.actorPrefab.gameObject.SetActive(false);
            
            var actor = Instantiate(actorDefinition.actorPrefab, position, Quaternion.identity, null);

            if (actor.TryGetComponent<GameplayStateController>(out var gameState))
                gameState.Construct(state);

            if (actor.TryGetComponent<Movement>(out var movement))
                movement.Construct(level);

            if (actor.TryGetComponent<CombatTarget>(out var target))
            {
                target.SetTarget(initialTarget);
            }
                

            if (actor.TryGetComponent<ParticlePool>(out var particlePool))
                particlePool.Construct(deathParticlePool);
            
            if(actor.TryGetComponent<ActorSpawnDelay>(out var spawnDelay))
                spawnDelay.Construct(deathParticlePool);
            
            if(actor.TryGetComponent<MunitionPool>(out var bulletPool))
                bulletPool.Construct(munitionPool);
            
            if(actor.TryGetComponent<SoundProxy>(out var actorSoundProxy))
                actorSoundProxy.Construct(soundManager);
            
            actor.gameObject.SetActive(true);

            return actor;
        }
    }
}

