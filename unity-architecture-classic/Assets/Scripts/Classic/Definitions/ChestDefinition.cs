using Classic.Items;
using UnityEngine;

namespace Classic.Definitions
{
    [CreateAssetMenu(fileName = "ChestDefinition", menuName = "Classic/ChestDefinition")]
    public class ChestDefinition : ScriptableObject
    {
        public Chest chestPrefab;
    }
}