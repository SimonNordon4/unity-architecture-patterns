using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreMenuManager : MonoBehaviour
{
    public RectTransform StoreItemContainer;
    public StoreItemUI StoreItemUIPrefab;
    public List<StoreItemUI> StoreItemUis = new List<StoreItemUI>();
    public TextMeshProUGUI goldText;

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
        foreach (var storeItem in AccountManager.instance.storeItems)
        {
            var storeItemUi = Instantiate(StoreItemUIPrefab, StoreItemContainer);
            storeItemUi.Initialize(storeItem);
            StoreItemUis.Add(storeItemUi);
        }
        goldText.text =$"GOLD: {AccountManager.instance.totalGold.ToString()}";
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
