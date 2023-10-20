using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "SpawnAction", menuName = "Classic/SpawnAction", order = 1)]
    public class SpawnAction : ScriptableObject
    {
        [field:SerializeField]public EnemyDefinition definition { get; private set; }
        [field:SerializeField]public int numberOfEnemiesToSpawn { get; private set; } = 1;
    }
}