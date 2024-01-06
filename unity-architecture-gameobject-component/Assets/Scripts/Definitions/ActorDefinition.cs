using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "ActorDefinition", menuName = "Classic/ActorDefinition", order = 1)]
    public class ActorDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        public PoolableActor actorPrefab;
        public Color enemyColor;
    }
}