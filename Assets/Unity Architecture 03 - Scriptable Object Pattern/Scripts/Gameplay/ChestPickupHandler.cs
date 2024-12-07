using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(ChestSpawner))]
    public class ChestPickupHandler : MonoBehaviour
    {
        public UnityEvent<Chest> OnChestPickup = new();
        
        [SerializeField]private GameState gameState;
        [SerializeField]private Inventory playerInventory;
        
        private ChestSpawner _chestSpawner;
        public Chest CurrentChest { get; private set; } = null;


        private void Awake()
        {
            _chestSpawner= GetComponent<ChestSpawner>();
        }

        private void OnEnable()
        {
            _chestSpawner.OnChestSpawned.AddListener(OnChestSpawned);
        }

        private void OnChestSpawned(Chest chest)
        {
            chest.OnChestOpened.AddListener(PickupChest);
        }

        private void PickupChest(Chest chest)
        {
            chest.OnChestOpened.RemoveListener(PickupChest);
            gameState.PauseGame();
            CurrentChest = chest;
            OnChestPickup.Invoke(chest);
        }

        public void SelectItem(ChestItem item)
        {
            playerInventory.AddItem(item);
            StartCoroutine(WaitOneFrameToUnpause());
        }
        
        private IEnumerator WaitOneFrameToUnpause()
        {
            yield return new WaitForEndOfFrame();
            gameState.ResumeGame();
            //Destroy(CurrentChest.gameObject);
        }

    }
}