using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class ChestItemButton : MonoBehaviour
    {
        public ChestItem chestItem;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI DescriptionPrefab;
        public RectTransform DescriptionContainer;
        public Image[] borders;
        
        private List<TextMeshProUGUI> _descriptions = new List<TextMeshProUGUI>();

        public void Initialize(ChestItem item)
        {
            foreach (var description in _descriptions)
            {
                Destroy(description.gameObject);
            }
            _descriptions.Clear();
            
            Title.text = item.itemName;
            chestItem = item;

            foreach (var border in borders)
            {
                border.color = item.tier switch
                {
                    1 => new Color(0.7f,0.7f,0.7f),
                    2 => new Color(0.5f,1f,0.3f),
                    3 => new Color(0.5f,0.5f,1f),
                    4 => new Color(0.8f,0.5f,0.8f),
                    5 => new Color(1f,0.5f,0.5f),
                    _ => new Color(0.5f,0.5f,0.5f),
                };
            }

            foreach (var mod in chestItem.modifiers)
            {
                // create a new description text
                var description = Instantiate(DescriptionPrefab, DescriptionContainer);
                
                    var statSign = mod.modifierValue > 0 ? "+" : "-";                

                    // Format stat value.
                    var statValueString = mod.modifierType != ModifierType.Percentage ?
                        statSign + mod.modifierValue :
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
                    
                    description.text = statValueString + " " + statTypeString;
                    // make the text green
                    description.color = mod.modifierValue > 0 ?
                        new Color(0.75f, 1, 0.75f):
                        new Color(1, 0.75f, 0.75f);

                _descriptions.Add(description);
            }
        }

        public void ApplyItem()
        {
            GameManager.instance.ApplyItem(chestItem);
            
        }
    }
