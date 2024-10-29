using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class Stat
    {
        public Stat(float initialValue)
        {
            baseValue = initialValue;
            value = baseValue;
        }

        public float baseValue = 1f;
        public float minimumValue = 0f;
        public float maximumValue = float.PositiveInfinity;

        [SerializeField] private List<Modifier> _modifiers = new();

        public float value = 1f;

        private void Evaluate()
        {
            float addedValue = 0;

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

            value = Mathf.Clamp((baseValue + addedValue), minimumValue, maximumValue);
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