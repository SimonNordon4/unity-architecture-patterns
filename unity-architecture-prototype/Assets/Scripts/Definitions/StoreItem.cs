using System;
using UnityEngine;


    [Serializable]
    public class StoreItem
    {
        public StatType type;
        public string name;
        public Sprite sprite;
        public Modifier[] tierModifiers;
        public int[] pricePerTier;
        public int currentTier;
        
        public StoreItemDefinition definition;

        public StoreItem(StoreItemDefinition def)
        {
            type = def.type;
            name = def.name;
            sprite = def.sprite;
            tierModifiers = def.tierModifiers;
            pricePerTier = def.pricePerTier;
            currentTier = 0;
        }
    }
