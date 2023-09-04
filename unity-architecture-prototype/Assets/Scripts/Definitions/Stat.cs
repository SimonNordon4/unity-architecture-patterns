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
    public float minimumValue = 0f;
    public float maximumValue = float.PositiveInfinity;
    
    [SerializeField]
    private List<Modifier> _modifiers = new();

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

                // We clamp the flatSum.
                // This improves gameplay, player can take a heavy penalty early (-8 damage),
                // but if they only have 2 damage, then the penalty is only -2.
                if (flatSum < minimumValue)
                {
                    flatSum = minimumValue;
                }
                
            }
            else if (modifier.modifierType == ModifierType.Percentage)
            {
                // Convert percentage to a multiplier. e.g., 10% becomes 1.10, -20% becomes 0.80
                percentageSum += modifier.modifierValue;
            }
        }

        value = Mathf.Clamp((initialValue + flatSum) * (percentageSum), minimumValue, maximumValue);
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
