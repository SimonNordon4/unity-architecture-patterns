using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class ChestItems : MonoBehaviour
    {
        public string chestTableName = "ChestItems";
        public int tier = 1;

        public List<ChestItem> chestItems = new();

        public void OnValidate()
        {
            foreach (var chestItem in chestItems)
            {
                chestItem.tier = tier;
            }
        }
    }
}