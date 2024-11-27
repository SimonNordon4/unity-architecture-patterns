using System;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "StoreItemDefinition", menuName = "GameObjectComponent/StoreItemDefinition")]
    public class StoreItemDefinition : ScriptableObject
    {
        public Sprite storeSprite;
        public string storeName = "New Item";
        public StoreItemUpgrade[] upgrades;
    }

    [Serializable]
    public class StoreItemUpgrade
    {
        public Modifier modifier;
        public int cost;
    }
    
    [Serializable]
    public class StoreItem
    {
        public int currentUpgrade;
        public StoreItemUpgrade[] upgrades;
        public string storeName;
    }
}