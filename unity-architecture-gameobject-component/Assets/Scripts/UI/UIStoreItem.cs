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
        private StoreItemDefinition _definition;

        public void Construct(Store store, Gold gold, StoreItem item)
        {
            _store = store;
            _gold = gold;
            _item = item;
            _definition = item.storeItemDefinition;
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
            itemImage.sprite = _definition.storeSprite;
            itemNameText.text = _definition.name;
        }

        private void UpdateCurrentModifierText()
        {
            itemCurrentModifierText.text = "";
            
            if (_item.upgradesPurchased <= 0)
            {
                return;
            }
            
            var currentModifier = _definition.upgrades[_item.upgradesPurchased - 1].modifier;
            
            if(_item.upgradesPurchased >= _definition.upgrades.Length)
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
            
            if(_item.upgradesPurchased >= _definition.upgrades.Length)
            {
                itemNextModifierText.color = inActiveColor;
                return;
            }
            
            var nextModifier = _definition.upgrades[_item.upgradesPurchased].modifier;
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
            
            for(var i = 0; i < _definition.upgrades.Length; i++)
            {
                var tierIndicator = Instantiate(tierIndicatorPrefab, tierIndicatorContainer);
                tierIndicator.transform.GetChild(0).gameObject.SetActive(i < _item.upgradesPurchased);
                tierIndicatorInactive.Add(tierIndicator);
            }
        }

        private void UpdateAffordability(int addedGold )
        {
            if(_item.upgradesPurchased == _definition.upgrades.Length)
            {
                purchaseButton.interactable = false;
                itemPriceText.text = "MAX";
                itemPriceText.color = inActiveColor;
                itemNextModifierText.color = inActiveColor;
                return;
            }
            var cost = _definition.upgrades[_item.upgradesPurchased].cost;
            
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