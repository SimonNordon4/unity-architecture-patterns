using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using GameObjectComponent.Pools;
using GameplayComponents.Actor;
using GameplayComponents.Combat;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace Pools
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [field:SerializeField] public Transform initialTarget { get; private set; }
        [SerializeField] private ParticlePool deathParticlePool;

        public PoolableActor Create(ActorDefinition actorDefinition, Vector3 position = new(), bool startActive = true)
        {
            actorDefinition.actorPrefab.gameObject.SetActive(false);
            
            var enemy = Instantiate(actorDefinition.actorPrefab, position, Quaternion.identity, null);
            
            if (enemy.TryGetComponent<ActorState>(out var gameState))
                gameState.Construct(state);

            if (enemy.TryGetComponent<Movement>(out var movement))
                movement.Construct(level);

            if (enemy.TryGetComponent<CombatTarget>(out var target))
                target.SetTarget(initialTarget);

            if (enemy.TryGetComponent<ParticlePool>(out var particlePool))
                particlePool.Construct(deathParticlePool);
            
            if(enemy.TryGetComponent<ActorSpawnDelay>(out var spawnDelay))
                spawnDelay.Construct(deathParticlePool);
            
            enemy.gameObject.SetActive(true);

            return enemy;
        }
    }
}

