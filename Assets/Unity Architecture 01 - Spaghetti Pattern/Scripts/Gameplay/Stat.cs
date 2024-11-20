using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class Stat
    {
        public Stat(int initialValue)
        {
            baseValue = initialValue;
            value = baseValue;
        }

        public int baseValue = 1;
        public int minimumValue = 0;
        public int maximumValue = int.MaxValue;

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

            var scaledValue = baseValue + baseValue * scalingPercentage;
            value = Mathf.Clamp(scaledValue + flatPercentage, minimumValue, maximumValue);
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