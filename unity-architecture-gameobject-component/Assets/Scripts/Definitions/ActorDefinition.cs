using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Classic/ActorDefinition", order = 1)]
    public class ActorDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        public PoolableActor poolableEnemyPrefab;
        public Color enemyColor;
        public ActorStatsDefinition statsDefinition;
    }
}