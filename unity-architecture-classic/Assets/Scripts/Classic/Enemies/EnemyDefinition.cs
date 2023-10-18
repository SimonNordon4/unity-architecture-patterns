using Classic.Enemies.Enemy;
using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Classic/EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        public GameObject enemyPrefab;
        public Color enemyColor;
    }
}