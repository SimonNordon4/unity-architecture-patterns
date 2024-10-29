using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityArchitecture.SpaghettiPattern
{
    [Serializable]
    public class ChestItems
    {
        public int tier = 1;
        public List<ChestItem> chestItems = new();

        public void SetTier()
        {
            foreach (var chestItem in chestItems)
            {
                chestItem.tier = tier;
            }
        }
    }
}