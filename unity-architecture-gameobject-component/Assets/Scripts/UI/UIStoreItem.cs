using System.Collections.Generic;
using GameObjectComponent.App;
using GameObjectComponent.Definitions;
using GameObjectComponent.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIStoreItem : MonoBehaviour
    {
        [field:SerializeField] public Button purchaseButton { get; private set; }
        
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemPriceText;
        [SerializeField] private TextMeshProUGUI itemCurrentModifierText;
        [SerializeField] private TextMeshProUGUI itemNextModifierText;

        [SerializeField] private GameObject tierIndicatorPrefab;
        [SerializeField] private RectTransform tierIndicatorContainer;
        [SerializeField] private List<GameObject> tierIndicatorInactive = new();
        [SerializeField] private Color inActiveColor;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color noMoneyColor;
        [SerializeField] private Color increaseColor;

                
        private Store _store;
        private Gold _gold;
        private StoreItem _item;

        public void Construct(Store store, Gold gold, StoreItem item)
        {
            _store = store;
            _gold = gold;
            _item = item;
            gold.OnGoldChanged += UpdateAffordability;
        }


        public void Init()
        {
            Reset();
            LoadDefinition();
            UpdateCurrentModifierText();
            UpdateNextModifierText();
            UpdateTierIndicators();
            UpdateAffordability(0);
            purchaseButton.onClick.AddListener(PurchaseUpgrade);
        }

        private void Refresh()
        {
            UpdateCurrentModifierText();
            UpdateNextModifierText();
            UpdateTierIndicators();
            UpdateAffordability(0);
        }
        
        public void PurchaseUpgrade()
        {
            _store.PurchaseUpgrade(_item);
            Refresh();
        }

        private void Reset()
        {
            purchaseButton.onClick.RemoveAllListeners();
            itemNameText.text = "";
            itemPriceText.text = "";
            purchaseButton.enabled = true;
            itemPriceText.color = Color.white;
            
            foreach (var tierIndicator in tierIndicatorInactive)
            {
                Destroy(tierIndicator);
            }
            tierIndicatorInactive.Clear();
        }

        private void LoadDefinition()
        {
            itemImage.sprite = _item.storeSprite;
            itemImage.color = Color.white;
            itemNameText.text = _item.storeName;
        }

        private void UpdateCurrentModifierText()
        {
            itemCurrentModifierText.text = "";
            
            if (_item.currentUpgrade <= 0)
            {
                return;
            }
            
            var currentModifier = _item.upgrades[_item.currentUpgrade - 1].modifier;
            
            if(_item.currentUpgrade >= _item.upgrades.Length)
            {
                itemCurrentModifierText.text = SurvivorsUtil.FormatModifierValue(currentModifier);
                itemCurrentModifierText.color = activeColor;
                return;
            }

            itemCurrentModifierText.text = SurvivorsUtil.FormatModifierValue(currentModifier);
            itemCurrentModifierText.color = increaseColor;
        }

        private void UpdateNextModifierText()
        {
            itemNextModifierText.text = "";
            
            if(_item.currentUpgrade >= _item.upgrades.Length)
            {
                itemNextModifierText.color = inActiveColor;
                return;
            }
            
            var nextModifier = _item.upgrades[_item.currentUpgrade].modifier;
            itemNextModifierText.text = SurvivorsUtil.FormatModifierValue(nextModifier);
            itemNextModifierText.color = increaseColor;
        }
        

        private void UpdateTierIndicators()
        {
            foreach (var tierIndicator in tierIndicatorInactive)
            {
                Destroy(tierIndicator);
            }
            
            tierIndicatorInactive.Clear();
            
            for(var i = 0; i < _item.upgrades.Length; i++)
            {
                var tierIndicator = Instantiate(tierIndicatorPrefab, tierIndicatorContainer);
                tierIndicator.transform.GetChild(0).gameObject.SetActive(i < _item.currentUpgrade);
                tierIndicatorInactive.Add(tierIndicator);
            }
        }

        private void UpdateAffordability(int addedGold )
        {
            if(_item.currentUpgrade == _item.upgrades.Length)
            {
                purchaseButton.interactable = false;
                itemPriceText.text = "MAX";
                itemPriceText.color = inActiveColor;
                itemNextModifierText.color = inActiveColor;
                return;
            }
            var cost = _item.upgrades[_item.currentUpgrade].cost;
            
            itemPriceText.text = cost + "G";
            
            if (_gold.amount >= cost)
            {
                itemPriceText.color = activeColor;
                purchaseButton.interactable = true;
                return;
            }

            purchaseButton.interactable = false;
            itemPriceText.color = noMoneyColor;
        }
    }
}