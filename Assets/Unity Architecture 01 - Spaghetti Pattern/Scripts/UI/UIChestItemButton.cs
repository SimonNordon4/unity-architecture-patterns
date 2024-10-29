using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIChestItemButton : MonoBehaviour
    {
        public Button selectItemButton;
        public ChestItem chestItem;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI DescriptionText;
        public RectTransform DescriptionContainer;
        public Image borderImage;
        public Sprite[] borderTiers;
        public Image itemSprite;
        
        private readonly List<TextMeshProUGUI> _descriptions = new();

        public void Initialize(ChestItem item, UIChestItemMenu chestMenu)
        {
            foreach (var description in _descriptions)
            {
                Destroy(description.gameObject);
            }
            _descriptions.Clear();
            
            Title.text = item.itemName;
            chestItem = item;
            
            itemSprite.sprite = item.sprite;

            borderImage.sprite = borderTiers[item.tier - 1];
            DescriptionText.gameObject.SetActive(true);

            foreach (var mod in chestItem.modifiers)
            {
                // create a new description text
                var description = Instantiate(DescriptionText, DescriptionContainer);
                
                // we don't need a minus because negative values will already have a minus
                    var statSign = mod.modifierValue > 0 ? "+" : "";                

                    // Format stat value.
                    var statValueString = statSign + mod.modifierValue;
          

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
                    
                    description.text = statValueString + " " + statTypeString;
                    // make the text green
                    description.color = mod.modifierValue > 0 ?
                        new Color(0.75f, 1, 0.75f):
                        new Color(1, 0.75f, 0.75f);

                _descriptions.Add(description);
            }
            selectItemButton.onClick.AddListener(() => chestMenu.OnItemSelected(chestItem));

            DescriptionText.gameObject.SetActive(false);
        }
    }
}