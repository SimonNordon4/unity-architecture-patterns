using UnityEngine;

namespace Classic.Core
{
    public class ChestDefinition : ScriptableObject
    {
        [field:SerializeField] 
        public Vector2Int tiers { get; private set; }= new(1, 5);
        [field:SerializeField] 
        public Vector2Int options { get; private set; } = new(2, 5);
        [field:SerializeField] 
        public ChestType chestType { get; private set; } = ChestType.Mini;
    }
}