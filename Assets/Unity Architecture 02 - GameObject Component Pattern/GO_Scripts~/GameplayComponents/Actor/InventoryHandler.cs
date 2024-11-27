using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameplayComponents.Actor
{
    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField]private Inventory inventory;
        [SerializeField]private Stats stats;
        
        private void OnEnable()
        {
            inventory.OnItemAdded += OnItemAdded;
        }
        
        private void OnDisable()
        {
            inventory.OnItemAdded -= OnItemAdded;
        }

        private void OnItemAdded(ChestItemDefinition chestItem)
        {
            foreach (var mod in chestItem.modifiers)
            {
                var stat = stats.GetStat(mod.statType);
                if (stat == null) continue;
                stat.AddModifier(mod);
            }
        }
    }
}