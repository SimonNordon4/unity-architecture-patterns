using System;
namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class Modifier
    {
        public StatType statType;
        public int modifierValue = 0;
        // Whether to be displayed as a percentage in the UI.
        public bool isPercentage = false;
    }
}