using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyStats : MonoBehaviour
    {
        [field: SerializeField] public int health { get; private set; } = 6;
        [field: SerializeField] public int damage { get; private set; } = 1;
        [field: SerializeField] public float speed { get; private set; } = 4;
        [field:SerializeField] public Vector3 velocity { get; set; }
    }
}