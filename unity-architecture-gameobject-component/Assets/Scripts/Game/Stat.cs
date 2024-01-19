using System;
using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

[Serializable]
public class Stat
{
    public event Action<Modifier> onModifierAdded;
    public event Action onStatChanged;
    
    public Stat(StatType newType)
    {
        type = newType;
        initialValue = 1f;
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
    public float initialValue;
    public float minimumValue;
    public float maximumValue;
    
    public StatType type;

    [field: SerializeField] public List<Modifier> modifiers { get; private set; } = new();

    [SerializeField]
    private float realValue;

    public float value
    {
        get => realValue;
        set
        {
            var originalValue = realValue;
            realValue = Mathf.Clamp(value, minimumValue, maximumValue);
            if (Math.Abs(realValue - originalValue) > 0.01f)
            {
                onStatChanged?.Invoke();
            }
        }
    }

    private void Evaluate()
    {
        float originalValue = initialValue;
        float flatSum = 0;
        float percentageSum = 1; // Start with 1 so it represents 100% at start.

        foreach (var modifier in modifiers)
        {
            switch (modifier.modifierType)
            {
                case ModifierType.Flat:
                {
                    flatSum += modifier.modifierValue;
                    if(flatSum + initialValue < minimumValue)
                    {
                        var virtualFlatSum = initialValue + flatSum;
                        var difference = minimumValue - virtualFlatSum;
                        var reNormalizedFlatSum = difference + flatSum;
                        flatSum = reNormalizedFlatSum;
                    }

                    break;
                }
                case ModifierType.Percentage:
                    // Convert percentage to a multiplier. e.g., 10% becomes 1.10, -20% becomes 0.80
                    percentageSum += modifier.modifierValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        value = Mathf.Clamp((initialValue + flatSum) * (percentageSum), minimumValue, maximumValue);
    }
    public void Reset()
    {
        modifiers.Clear();
        value = initialValue;
    }
    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        Evaluate();
        onModifierAdded?.Invoke(modifier);
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
