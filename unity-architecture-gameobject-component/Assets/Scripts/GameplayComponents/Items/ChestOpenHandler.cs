using GameObjectComponent.Game;
using GameObjectComponent.UI;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class ChestOpenHandler : MonoBehaviour
    {
        // [SerializeField] private GameState gameState;
        // [SerializeField] private UIState uiState;
        // [SerializeField] private Inventory inventory;
        //
        // public Chest currentChest { get; private set; } = null;
        //
        // private void OnEnable()
        // {
        //     Chest.OnChestPickedUp.AddListener(OnChestPickedUp);
        // }
        //
        // private void OnChestPickedUp(Chest chest)
        // {
        //     currentChest = chest;
        //     gameState.PauseGame();
        //     uiState.GoToChestMenu();
        // }
        //
        // public void SelectItem(ChestItem item)
        // {
        //     Debug.Log($"Selected {item.name}");
        //     gameState.ResumeGame();
        //     uiState.GoToHud();
        //     
        //     inventory.AddChestItem(item);
        //     
        //     Destroy(currentChest.gameObject);
        //     currentChest = null;
        // }
    }
}