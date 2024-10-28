using System;
namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class Modifier
    {
        public StatType statType;
        public ModifierType modifierType = ModifierType.Flat;
        public float modifierValue = 0f;
    }
}