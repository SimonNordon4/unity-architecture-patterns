using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "SpawnAction", menuName = "Classic/SpawnAction", order = 1)]
    public class SpawnActionDefinition : ScriptableObject
    {
        [field:SerializeField]public EnemyDefinition definition { get; private set; }
        [field:SerializeField]public int numberOfEnemiesToSpawn { get; private set; } = 1;
        [field:SerializeField]public SpawnActionType actionType { get; private set; } = SpawnActionType.Group;
    }
}