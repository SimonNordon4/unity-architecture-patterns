using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// 1h 25m min
// TODO:
// We need to completely fix the serialization of store items, split into definitions and data - or just skip load with a bool.
// Fix store item indicators as well.

[CreateAssetMenu(fileName = "New Item", menuName = "Prototype/Store Item")]
public class StoreItemDefinition : ScriptableObject
{
    public ModifierType modifierType = ModifierType.Flat;
    public StatType type = StatType.EnemySpawnRate;
    
    public string name = "New Item";
    public Sprite sprite;
    public Modifier[] tierModifiers;
    public int[] pricePerTier;

    private void OnValidate()
    {
        var n = type.ToString();
        // add a space inbetween every capital letter
        n = System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        if(modifierType == ModifierType.Percentage)
            n += " %";

        name = n;
        foreach(var mod in tierModifiers)
        {mod.statType = type;}
    }
}


