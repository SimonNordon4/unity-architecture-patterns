using System.Collections.Generic;
using Classic.Game;
using TMPro;
using UnityEngine;

public class StoreMenuManager : MonoBehaviour
{
    public RectTransform StoreItemContainer;
    public StoreItemUI StoreItemUIPrefab;
    public List<StoreItemUI> StoreItemUis = new List<StoreItemUI>();
    public TextMeshProUGUI goldText;

    [SerializeField] private Inventory inventory;

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
        foreach (var storeItem in inventory.storeItems)
        {
            var storeItemUi = Instantiate(StoreItemUIPrefab, StoreItemContainer);
            storeItemUi.Initialize(storeItem);
            StoreItemUis.Add(storeItemUi);
            buttons.Add(storeItemUi.purchaseButton);
        }
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
    
    private void OnValidate()
    {
        if (inventory == null)
        {
            inventory = FindObjectsByType<Inventory>(FindObjectsSortMode.None)[0];
        }
    }
}
