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
            int addedValue = 0;

            foreach (var modifier in _modifiers)
            {
                addedValue += modifier.modifierValue;

                if (addedValue + baseValue < minimumValue)
                {
                    var virtualFlatSum = baseValue + addedValue;
                    var difference = minimumValue - virtualFlatSum;
                    var reNormalizedFlatSum = difference + addedValue;
                    addedValue = reNormalizedFlatSum;
                }
            }

            value = Mathf.Clamp((int)(baseValue + addedValue), minimumValue, maximumValue);
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