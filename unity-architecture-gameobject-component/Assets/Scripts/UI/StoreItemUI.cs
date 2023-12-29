using System.Collections.Generic;
using GameObjectComponent.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class StoreItemUI : MonoBehaviour
    {
        [SerializeField] private Gold gold;
        
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemPriceText;
        public TextMeshProUGUI itemCurrentModifierText;
        public TextMeshProUGUI itemNextModifierText;
        public Button purchaseButton;


        public GameObject tierIndicatorPrefab;
        public RectTransform tierIndicatorContainer;
        public List<GameObject> tierIndicatorInactive;
        public Color inActiveColor;
        public Color activeColor;

        public Color noMoneyColor;


        public void Initialize(StoreItem item)
        {
            Debug.Log("Initializing store item ui for: " + item.name);
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

            if (item.sprite == null)
            {
                Debug.LogError("Missing sprite?");
            }
            
            itemImage.sprite = item.sprite;
            itemNameText.text = item.name;

            // We've hit the Maximum tier, no more upgrades are available.
            if (item.currentTier == item.pricePerTier.Length)
            {
                itemCurrentModifierText.text = FormatModifierValue(item.tierModifiers[item.currentTier-1]);
                itemNextModifierText.text = "MAX";
                purchaseButton.enabled = false;
                itemPriceText.text = "MAX";
            }
            else
            {
                var itemCost = item.pricePerTier[item.currentTier];
                itemPriceText.text = (int)itemCost + "G";
            
                var mod = item.tierModifiers[item.currentTier];
                    
                itemNextModifierText.text += FormatModifierValue(mod)  + "\n";
                itemNextModifierText.color = mod.modifierValue > 0 ?
                    new Color(0.75f, 1, 0.75f):
                    new Color(1, 0.75f, 0.75f);

                if (item.currentTier > 0)
                {
                    itemCurrentModifierText.text += FormatModifierValue(item.tierModifiers[item.currentTier - 1]) + "\n";
                    itemCurrentModifierText.color = item.tierModifiers[item.currentTier - 1].modifierValue > 0 ?
                        new Color(0.75f, 1, 0.75f):
                        new Color(1, 0.75f, 0.75f);
                }

                if (gold.amount < item.pricePerTier[item.currentTier])
                {
                    purchaseButton.interactable = false;
                    itemPriceText.color = noMoneyColor;
                    itemNextModifierText.color = inActiveColor;
                    
                }

                purchaseButton.onClick.AddListener(() =>
                {
                    Debug.Log("Adding item listener for: "  + item.name);
                    AccountManager.instance.PurchaseStoreItem(item);
                });
            }
            
            // create
            for(var i = 0; i < item.pricePerTier.Length; i++)
            {
                var tierIndicator = Instantiate(tierIndicatorPrefab, tierIndicatorContainer);
                
                if(i < item.currentTier)
                    tierIndicator.transform.GetChild(0).gameObject.SetActive(true);
                else
                    tierIndicator.transform.GetChild(0).gameObject.SetActive(false);
                
                tierIndicatorInactive.Add(tierIndicator);
            }
        }
        
        private string FormatModifierValue(Modifier mod)
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
            
            return statValueString + " " + statTypeString;
        }

        private void OnValidate()
        {
            if (gold == null)
            {
                gold = FindObjectsByType<Gold>(FindObjectsSortMode.None)[0];
            }
        }
    }
