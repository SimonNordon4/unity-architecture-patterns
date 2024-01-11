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
        [SerializeField] private List<GameObject> tierIndicatorInactive;
        [SerializeField] private Color inActiveColor;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color noMoneyColor;

        private StoreItem _currentItem;

        public void UpdateAffordability(int availableGold)
        {
            // this will be gold.
            if (availableGold >= _currentItem.storeItemDefinition.upgrades[_currentItem.upgradesPurchased].cost) return;
            purchaseButton.interactable = false;
            itemPriceText.color = noMoneyColor;
            itemNextModifierText.color = inActiveColor;
        }
        
        public void Initialize(StoreItem item, int availableGold)
        {
            _currentItem = item;
            var definition = item.storeItemDefinition;
            Debug.Log("Initializing store item ui for: " + item.storeItemDefinition.name);
            purchaseButton.onClick.RemoveAllListeners();
            // reset values
            itemNameText.text = "";
            itemPriceText.text = "";
            purchaseButton.enabled = true;
            itemPriceText.color = Color.white;
            
            foreach (var tierIndicator in tierIndicatorInactive)
            {
                Destroy(tierIndicator);
            }
            tierIndicatorInactive.Clear();
            
            // Create

            itemImage.color = Color.white;

            if (definition.storeSprite == null)
            {
                Debug.LogError("Missing sprite?");
            }
            
            itemImage.sprite = definition.storeSprite;
            itemNameText.text = definition.name;
            
            var currentUpgrade = definition.upgrades[item.upgradesPurchased];

            // We've hit the Maximum tier, no more upgrades are available.
            if (item.upgradesPurchased == definition.upgrades.Length)
            {
                itemCurrentModifierText.text = SurvivorsUtil.FormatModifierValue(currentUpgrade.modifier);
                itemNextModifierText.text = "MAX";
                purchaseButton.enabled = false;
                itemPriceText.text = "MAX";
            }
            else
            {
                itemPriceText.text = currentUpgrade.cost + "G";
            
                var mod = currentUpgrade.modifier;
                    
                itemNextModifierText.text += SurvivorsUtil.FormatModifierValue(currentUpgrade.modifier)  + "\n";
                itemNextModifierText.color = mod.modifierValue > 0 ?
                    new Color(0.75f, 1, 0.75f):
                    new Color(1, 0.75f, 0.75f);

                if (item.upgradesPurchased > 0)
                {
                    itemCurrentModifierText.text += SurvivorsUtil.FormatModifierValue(definition.upgrades[item.upgradesPurchased - 1].modifier) + "\n";
                    itemCurrentModifierText.color = currentUpgrade.modifier.modifierValue > 0 ?
                        new Color(0.75f, 1, 0.75f):
                        new Color(1, 0.75f, 0.75f);
                }
            }
            
            UpdateAffordability(availableGold);
            
            // create
            for(var i = 0; i < definition.upgrades.Length; i++)
            {
                var tierIndicator = Instantiate(tierIndicatorPrefab, tierIndicatorContainer);
                tierIndicator.transform.GetChild(0).gameObject.SetActive(i < item.upgradesPurchased);
                tierIndicatorInactive.Add(tierIndicator);
            }
        }
        
        
    }
}