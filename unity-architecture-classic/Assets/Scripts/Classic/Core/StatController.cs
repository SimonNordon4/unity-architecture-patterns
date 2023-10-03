using UnityEngine;

namespace Classic.Core
{
    public class StatController : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private Inventory inventory;
        [SerializeField] private Stats stats;
        
        private void OnEnable()
        {
            gameState.onGameStart.AddListener(GameStarted);
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

        private void OnValidate()
        {
            if (inventory == null)
            {
                inventory = FindObjectsByType<Inventory>(FindObjectsSortMode.None)[0];
            }

            if (stats == null)
            {
                stats = FindObjectsByType<Stats>(FindObjectsSortMode.None)[0];
            }
            
            if (gameState == null)
            {
                gameState = FindObjectsByType<GameState>(FindObjectsSortMode.None)[0];
            }
        }
    }
}