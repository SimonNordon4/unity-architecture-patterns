using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyStats : MonoBehaviour
    {
        public int baseHealth = 10;
        public float moveSpeed = 5;
        public int damage = 1;
        public float attackSpeed = 0.2f;
        public float attackRange = 0.5f;
        public float knockBackFactor = 1f;
        public Vector3 velocity;

        public void Initialize(EnemyDefinition definition)
        {
            baseHealth = definition.baseHealth;
            moveSpeed = definition.moveSpeed;
            damage = definition.damage;
            attackSpeed = definition.attackSpeed;
            attackRange = definition.attackRange;
        }
    }
}