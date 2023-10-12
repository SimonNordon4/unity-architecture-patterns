using Classic.Enemies.Enemy;
using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Classic/EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        public EnemyScope enemyPrefab;
        public SpawnIndicatorController spawnIndicatorPrefab;
        public ParticleSystem deathEffect;
        
        [Header("Base Stats")]
        public EnemyType enemyType;
        public int baseHealth = 10;
        public int damage = 1;
        public float moveSpeed = 5;
        public float attackSpeed = 0.2f;
        public float attackRange = 0.5f;
    }
}