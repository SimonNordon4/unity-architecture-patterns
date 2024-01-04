using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using GameObjectComponent.UI;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class ChestOpenHandler : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private UIState uiState;
        [SerializeField] private Inventory inventory;
        
        public Chest currentChest { get; private set; } = null;
        
        private void OnEnable()
        {
            Chest.OnChestPickedUp += OnChestPickedUp;
        }
        
        private void OnChestPickedUp(Chest chest)
        {
            currentChest = chest;
            gameState.PauseGame();
            uiState.GoToChestMenu();
        }
        
        public void SelectItem(ItemDefinition item)
        {
            Debug.Log($"Selected {item.name}");
            gameState.ResumeGame();
            uiState.GoToHud();
            inventory.AddItem(item);
            Destroy(currentChest.gameObject);
            currentChest = null;
        }
    }
}