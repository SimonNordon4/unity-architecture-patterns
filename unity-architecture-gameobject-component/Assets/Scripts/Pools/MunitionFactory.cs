using GameObjectComponent.Game;
using GameplayComponents.Actor;
using GameplayComponents.Combat;
using UnityEngine;

namespace Pools
{
    public class MunitionFactory : MonoBehaviour
    {
        [SerializeField]private GameState state;
        
        public Projectile Create(ProjectileDefinition projectileDefinition, Vector3 position = new(),Vector3 direction = new(), bool startActive = true)
        {
            projectileDefinition.prefab.gameObject.SetActive(false);
            var rotation = Quaternion.LookRotation(direction);
            var projectile = Instantiate(projectileDefinition.prefab, position, rotation, null);
            if (projectile.TryGetComponent<GameplayStateController>(out var gameState))
                gameState.Construct(state);

            projectile.gameObject.SetActive(true);

            return projectile;
        }
    }
}