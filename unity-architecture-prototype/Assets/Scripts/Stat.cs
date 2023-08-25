using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public Stat(float initialValue)
    {
        this.initialValue = initialValue;
        value = initialValue;
    }
    
    public float initialValue = 1f;
    
    private readonly List<Modifier> _modifiers = new();

    public float value = 1f;

    private void Evaluate()
    {
        float flatSum = 0;
        float percentageSum = 1; // Start with 1 so it represents 100% at start.

        foreach (var modifier in _modifiers)
        {
            if (modifier.modifierType == ModifierType.Flat)
            {
                flatSum += modifier.modifierValue;
            }
            else if (modifier.modifierType == ModifierType.Percentage)
            {
                // Convert percentage to a multiplier. e.g., 10% becomes 1.10, -20% becomes 0.80
                percentageSum += modifier.modifierValue / 100;
            }
        }

        value = (initialValue + flatSum) * percentageSum;
    }
    
    public void Reset()
    {
        _modifiers.Clear();
        value = initialValue;
    }
    
    public void AddModifier(Modifier modifier)
    {
        _modifiers.Add(modifier);
        Evaluate();
    }
}
