using Classic.Actors;
using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Classic/EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        public GameObject enemyPrefab;
        public Color enemyColor;
        public ActorStatsDefinition statsDefinition;
    }
}