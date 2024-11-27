using GameObjectComponent.Items;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "ChestDefinition", menuName = "Classic/ChestDefinition")]
    public class ChestDefinition : ScriptableObject
    {
        public Chest chestPrefab;
    }
}