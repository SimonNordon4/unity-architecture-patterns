using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [Serializable]
    public class Stat
    {
        public Stat(int initialValue, StatType statType)
        {
            baseValue = initialValue;
            value = baseValue;
            StatType = statType;
        }

        public readonly StatType StatType;
        [field:SerializeField] public int baseValue {get;private set;} = 1;
        [SerializeField] private int minimumValue = 0;
        [SerializeField] private int maximumValue = int.MaxValue;

        [SerializeField] private List<Modifier> _modifiers = new();

        public int value;

        private void Evaluate()
        {
            int scalingPercentage = 0;
            int flatPercentage = 0;

            foreach (var modifier in _modifiers)
            {
                if (modifier.isFlatPercentage)
                {
                    flatPercentage += modifier.modifierValue;
                }
                else
                {
                    scalingPercentage += modifier.modifierValue;
                }
            }
            var scaledValue = baseValue + baseValue * (scalingPercentage / 100f);
            value = (int)Mathf.Clamp(scaledValue + flatPercentage, minimumValue, maximumValue);
        }

        public void Reset()
        {
            _modifiers.Clear();
            value = baseValue;
        }

        public void AddModifier(Modifier modifier)
        {
            _modifiers.Add(modifier);
            Evaluate();
        }
    }
}