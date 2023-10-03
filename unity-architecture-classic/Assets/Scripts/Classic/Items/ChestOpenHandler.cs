using Classic.Core;
using Classic.UI;
using UnityEngine;

namespace Classic.Items
{
    public class ChestOpenHandler : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private UIState uiState;
        [SerializeField] private Inventory inventory;
        
        public Chest currentChest { get; private set; } = null;
        
        private void OnEnable()
        {
            Chest.OnChestPickedUp.AddListener(OnChestPickedUp);
        }

        private void OnChestPickedUp(Chest chest)
        {
            currentChest = chest;
            gameState.PauseGame();
            uiState.GoToChestMenu();
            
            // Temporary
            GameManager.instance.isGameActive = false;
        }

        public void SelectItem(ChestItem item)
        {
            Debug.Log($"Selected {item.name}");
            gameState.ResumeGame();
            uiState.GoToHud();
            
            inventory.AddChestItem(item);
            
            // Temporary
            GameManager.instance.ApplyItem(item);
            GameManager.instance.isGameActive = true;
            
            Destroy(currentChest.gameObject);
            currentChest = null;
        }

        private void OnValidate()
        {
            if (inventory == null)
            {
                inventory = FindObjectsByType<Inventory>(FindObjectsSortMode.None)[0];
            }

            if (gameState == null)
            {
                gameState = FindObjectsByType<GameState>(FindObjectsSortMode.None)[0];
            }
                

            if (uiState == null)
            {
                uiState = FindObjectsByType<UIState>(FindObjectsSortMode.None)[0];
            }
                
        }
    }
}