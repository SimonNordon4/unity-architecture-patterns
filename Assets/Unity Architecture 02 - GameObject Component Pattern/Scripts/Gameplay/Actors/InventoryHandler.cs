using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(Stats))]
    public class InventoryHandler : MonoBehaviour
    {
        private Inventory _inventory;
        private Stats _stats;

        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _stats = GetComponent<Stats>();
        }
        
        private void OnEnable()
        {
            _inventory.onItemAdded.AddListener(OnItemAdded);
        }
        
        private void OnDisable()
        {
            _inventory.onItemAdded.RemoveListener(OnItemAdded);
        }

        private void OnItemAdded(ChestItem chestItem)
        {
            foreach (var mod in chestItem.modifiers)
            {
                var stat = _stats.GetStat(mod.statType);
                stat.AddModifier(mod);
            }
        }
    }
}