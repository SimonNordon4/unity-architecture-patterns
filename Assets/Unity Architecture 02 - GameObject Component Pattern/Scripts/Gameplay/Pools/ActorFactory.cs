using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private Level level;
        [field:SerializeField] public Transform initialTarget { get; private set; }
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private PoolableActor actorPrefab;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private UserSettings userSettings;

        public PoolableActor Create(Vector3 position = new(), bool startActive = true)
        {
            actorPrefab.gameObject.SetActive(false);
            
            var actor = Instantiate(actorPrefab, position, Quaternion.identity, null);

            if (actor.TryGetComponent<Movement>(out var movement))
                movement.Construct(level);

            if (actor.TryGetComponent<CombatTarget>(out var target))
                target.SetTarget(initialTarget);
            
            if(actor.TryGetComponent<ProjectilePool>(out var bulletPool))
                bulletPool.Construct(projectilePool);
            
            if(actor.TryGetComponent<SoundProxy>(out var actorSoundProxy))
                actorSoundProxy.Construct(soundManager);

            UIActorHealthBar healthBar = actor.GetComponentInChildren<UIActorHealthBar>();
            
            if (healthBar != null)
            {
                healthBar.Construct(userSettings);
            }
            
            UIDamageNumber damageNumber = actor.GetComponentInChildren<UIDamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Construct(userSettings);
            }

            
            actor.gameObject.SetActive(true);

            return actor;
        }
    }
}

