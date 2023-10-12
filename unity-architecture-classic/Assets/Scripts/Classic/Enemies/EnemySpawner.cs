using UnityEngine;

namespace Classic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyDefinition _definition;
        [SerializeField] private EnemyPool pool;
    }
}