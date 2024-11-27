using GameObjectComponent.Game;
using GameplayComponents.Actor;
using GameplayComponents.Combat;
using UnityEngine;

namespace Pools
{
    public class MunitionFactory : MonoBehaviour
    {
        [SerializeField]private GameState state;
        
        public Munition Create(MunitionDefinition munitionDefinition, Vector3 position = new(),Vector3 direction = new(), bool startActive = true)
        {
            munitionDefinition.prefab.gameObject.SetActive(false);
            var rotation = Quaternion.LookRotation(direction);
            var munition = Instantiate(munitionDefinition.prefab, position, rotation, null);
            if (munition.TryGetComponent<GameplayStateController>(out var gameState))
                gameState.Construct(state);

            munition.gameObject.SetActive(true);

            return munition;
        }
    }
}