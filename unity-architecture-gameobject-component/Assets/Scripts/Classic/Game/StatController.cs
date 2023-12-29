using UnityEngine;

namespace Classic.Game
{
    public class StatController : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private Inventory inventory;
        [SerializeField] private Stats stats;
        
        private void OnEnable()
        {
            gameState.OnGameStart+=(GameStarted);
            inventory.onChestItemAdded.AddListener(OnChestItemAdded);
        }

        private void GameStarted()
        {
            stats.ResetModifiers();
            ApplyStoreItems();
        }

        private void ApplyStoreItems()
        {
            foreach (var storeItem in inventory.storeItems)
            {
                if(storeItem.currentTier == 0) continue;
                var storeModifier = storeItem.tierModifiers[storeItem.currentTier - 1];
                Debug.Log($"Applying store item {storeItem.name} with modifier {storeModifier}");
                stats.ApplyModifier(storeModifier);
            }
        }

        private void OnDisable()
        {
            inventory.onChestItemAdded.RemoveListener(OnChestItemAdded);
        }

        private void OnChestItemAdded(ChestItem chestItem)
        {
            stats.ApplyModifier(chestItem.modifiers);
        }
    }
}