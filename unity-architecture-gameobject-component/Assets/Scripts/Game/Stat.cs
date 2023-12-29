using System;
using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

[Serializable]
public class Stat
{
    public Stat(StatType newType)
    {
        type = newType;
        initialValue = 0f;
        minimumValue = 0f;
        maximumValue = float.PositiveInfinity;
        SetDefault();
        name = type.ToString();
        value = initialValue;
    }
    
    public Stat(Stat stat)
    {
        name = stat.name;
        initialValue = stat.initialValue;
        minimumValue = stat.minimumValue;
        maximumValue = stat.maximumValue;
        type = stat.type;
        name = stat.name;
        value = initialValue;
    }

    public string name;
    public float initialValue = 1f;
    public float minimumValue = 0f;
    public float maximumValue = float.PositiveInfinity;
    
    public StatType type;
    
    [SerializeField]
    private List<Modifier> _modifiers = new();

    public float value;

    private void Evaluate()
    {
        float flatSum = 0;
        float percentageSum = 1; // Start with 1 so it represents 100% at start.

        foreach (var modifier in _modifiers)
        {
            if (modifier.modifierType == ModifierType.Flat)
            {
                flatSum += modifier.modifierValue;
                
                // intialValue = 1.5, flatsum = -3, minimumValue = 1
                // actualValue = 1.5 - 3 = -1.5
                // difference = 1 - (-1.5) = 2.5
                // reNormalizedFlatSum = 2.5 + (-3) = -0.5
                
                // minimumValue = initialValue + flatSum + x
                // 1 = 2 + -3 + x

                // x = minimumValue - initialValue - flatSum
                // x = 1 - 2 -(-3)
                // x = 2
                
                // new flatsum += 2;

                if(flatSum + initialValue < minimumValue)
                {
                    var virtualFlatSum = initialValue + flatSum;
                    var difference = minimumValue - virtualFlatSum;
                    var reNormalizedFlatSum = difference + flatSum;
                    flatSum = reNormalizedFlatSum;
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

    private void SetDefault()
    {
        switch (type)
        {
            case StatType.RangedAttackSpeed:
                initialValue = 1f;
                break;
            case StatType.RangedDamage:
                initialValue = 1f;
                break;
            case StatType.RangedRange:
                initialValue = 5f;
                break;
            case StatType.RangedKnockBack:
                initialValue = 1f;
                break;
            case StatType.RangedPierce:
                initialValue = 0f;
                break;
            case StatType.MeleeDamage:
                initialValue = 1f;
                break;
            case StatType.MeleeKnockBack:
                initialValue = 1f;
                break;
            case StatType.MeleeRange:
                initialValue = 1.5f;
                break;
            case StatType.MeleeAttackSpeed:
                initialValue = 1f;
                break;
            case StatType.MoveSpeed:
                initialValue = 5f;
                break;
            case StatType.MaxHealth:
                initialValue = 5f;
                break;
            case StatType.HealthPackDropRate:
                initialValue = 0.1f;
                break;
            case StatType.Luck:
                initialValue = 0f;
                break;
            case StatType.Block:
                initialValue = 0f;
                break;
            case StatType.Dodge:
                initialValue = 0f;
                maximumValue = 60f;
                break;
            case StatType.Revives:
                initialValue = 0f;
                break;
            case StatType.Dashes:
                initialValue = 0f;
                break;
        }
    }
}
