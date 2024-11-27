using System;
namespace UnityArchitecture.GameObjectComponentPattern
{
    [Serializable]
    public class Modifier
    {
        public StatType statType;
        public int modifierValue = 0;
        public bool isFlatPercentage = false;
    }
}