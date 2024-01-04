using System.Collections.Generic;
using GameObjectComponent.Definitions;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTableDefinition", menuName = "GameObjectComponent/ItemTableDefinition", order = 1)]
public class ItemTableDefinition : ScriptableObject
{
    [field: SerializeField] public List<ItemDefinition> items { get; private set; } = new();
}