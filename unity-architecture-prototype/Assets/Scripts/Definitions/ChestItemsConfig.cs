using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestItemsConfig", menuName = "ScriptableObjects/ChestItemsConfig", order = 1)]
public class ChestItemsConfig : ScriptableObject
{
    public List<ChestItem> chestItems;
}

