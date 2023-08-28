using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class StoreItemUI : MonoBehaviour
    {
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemPriceText;
        public TextMeshProUGUI itemModifierText;
        public Button purchaseButton;


        public GameObject tierIndicatorPrefab;
        public RectTransform tierIndicatorContainer;
        public List<GameObject> tierIndicatorInactive;
        public Color inActiveColor;
        public Color activeColor;


        public void Initialize(StoreItem item)
        {
            purchaseButton.onClick.RemoveAllListeners();
            // reset values
            if(itemImage.sprite != null)
                itemImage.sprite = null;
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
            
            itemImage.sprite = item.sprite;
            itemNameText.text = item.itemName;
            var itemCost = item.price + item.currentTier * item.priceIncreasePerTier;
            itemPriceText.text = (int)itemCost + "G";
            
            foreach(var mod in item.modifiers)
            {
                var statSign = mod.modifierValue > 0 ? "+" : "-";                

                // Format stat value.
                var statValueString = mod.modifierType != ModifierType.Percentage ?
                    statSign + (mod.modifierValue) :
                    $"{statSign}{mod.modifierValue * 100}%";

                    
                // Format stat type name.
                var statTypeString = mod.statType.ToString();
                    
                for (var i = 1; i < statTypeString.Length; i++)
                {
                    if (char.IsUpper(statTypeString[i]))
                    {
                        statTypeString = statTypeString.Insert(i, " ");
                        i++;
                    }
                }

                statTypeString = statTypeString.ToLower();
                    
                itemModifierText.text += statValueString + " " + statTypeString + "\n";
                // make the text green
                itemModifierText.color = mod.modifierValue > 0 ?
                    new Color(0.75f, 1, 0.75f):
                    new Color(1, 0.75f, 0.75f);
            }
            
            
            Debug.Log("Total Gold: " + AccountManager.instance.totalGold + " Item Cost: " + itemCost + " Can Afford: " + (AccountManager.instance.totalGold >= itemCost));
            if (AccountManager.instance.totalGold < itemCost)
            {
                purchaseButton.enabled = false;
                itemPriceText.color = Color.red;
            }

            if (item.currentTier >= item.tiers)
            {
                purchaseButton.enabled = false;
                itemPriceText.text = "MAX";
            }
            
            purchaseButton.onClick.AddListener(() =>
            {
                AccountManager.instance.PurchaseStoreItem(item);
            });
            
            // create
            for(var i = 0; i < item.tiers; i++)
            {
                var tierIndicator = Instantiate(tierIndicatorPrefab, tierIndicatorContainer);
                tierIndicator.GetComponent<Image>().color = i < item.currentTier ? activeColor : inActiveColor;
                tierIndicatorInactive.Add(tierIndicator);
            }
            

        }
    }
