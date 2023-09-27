using Classic.Core;
using Classic.UI;
using UnityEngine;

namespace Classic.Items
{
    public class ChestOpenHandler : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private UIState uiState;
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
            
            // Temporary
            GameManager.instance.ApplyItem(item);
            GameManager.instance.isGameActive = true;
            
            Destroy(currentChest.gameObject);
            currentChest = null;
        }
    }
}