using System.Collections.Generic;
using GameObjectComponent.GameplayComponents.Combat;
using GameObjectComponent.Game;
using TMPro;
using UnityEngine;
using Inventory = GameObjectComponent.GameplayComponents.Combat.Inventory;

public class StoreMenuManager : MonoBehaviour
{
    public RectTransform StoreItemContainer;
    public StoreItemUI StoreItemUIPrefab;
    public List<StoreItemUI> StoreItemUis = new List<StoreItemUI>();
    public TextMeshProUGUI goldText;

    [SerializeField] private Inventory characterInventory;

    private void OnEnable()
    {
        Init();
    }
    
    public void UpdateStoreMenu()
    {
        Clear();
        Init();
    }

    void Init()
    {
        // Populate all the store item uis.
        var buttons = new List<UnityEngine.UI.Button>();
        // TODO: This needs to be from a list of store items.
        // foreach (var storeItem in characterInventory.storeItems)
        // {
        //     var storeItemUi = Instantiate(StoreItemUIPrefab, StoreItemContainer);
        //     storeItemUi.Initialize(storeItem);
        //     StoreItemUis.Add(storeItemUi);
        //     buttons.Add(storeItemUi.purchaseButton);
        // }
    }

    void Clear()
    {
        // Destroy all the store item uis.
        foreach (var storeItemUi in StoreItemUis)
        {
            if(storeItemUi != null)
                Destroy(storeItemUi.gameObject);
        }
        StoreItemUis.Clear();
    }
    
    private void OnDisable()
    {
        Clear();
    }
}
