using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// 1h 25m min
// TODO:
// We need to completely fix the serialization of store items, split into definitions and data - or just skip load with a bool.
// Fix store item indicators as well.
[Serializable]
public class StoreItem 
{
    public Sprite sprite;
    public string itemName = "New Item";
    public Modifier[] modifiers;
    public float tierModifierMultiplier = 1f;
    public int price = 100;
    public int tiers = 3;
    public float priceIncreasePerTier = 10f;
    public int currentTier = 0;
}


