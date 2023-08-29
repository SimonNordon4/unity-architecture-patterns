using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreItemConfig", menuName = "Prototype/StoreItemConfig", order = 1)]
public class StoreItemConfig : ScriptableObject
{
    public List<StoreItem> storeItems = new();
}
