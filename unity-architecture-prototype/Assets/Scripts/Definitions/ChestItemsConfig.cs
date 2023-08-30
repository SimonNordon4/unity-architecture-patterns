using System.Collections;
using System.Collections.Generic;
using UnityEngine;using Quaternion = System.Numerics.Quaternion;

[CreateAssetMenu(fileName = "ChestItemsConfig", menuName = "Prototype/ChestItemsConfig", order = 1)]
public class ChestItemsConfig : ScriptableObject
{
    public Quaternion rotation;
    public List<ChestItem> chestItems;
}

