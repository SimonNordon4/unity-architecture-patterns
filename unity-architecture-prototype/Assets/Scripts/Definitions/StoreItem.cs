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
    public string name = "New Item";
    public Sprite sprite;
    public int tiers = 3;
    public Modifier[] tierModifiers;
    public int[] pricePerTier;
    public int currentTier = 0;
    
    public StoreItem()
    {
        tierModifiers = new Modifier[tiers];
        pricePerTier = new int[tiers];
    }
}


