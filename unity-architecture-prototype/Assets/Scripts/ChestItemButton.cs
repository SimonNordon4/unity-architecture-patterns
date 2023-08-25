using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ChestItemButton : MonoBehaviour
    {
        public ChestItem chestItem;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI DescriptionPrefab;
        public RectTransform DescriptionContainer;
        public Image Background;
        
        private List<TextMeshProUGUI> _descriptions = new List<TextMeshProUGUI>();

        public void Initialize(ChestItem item)
        {
            foreach (var description in _descriptions)
            {
                Destroy(description.gameObject);
            }
            _descriptions.Clear();
            
            Title.text = item.name;
            chestItem = item;
            Background.color = item.tier switch
            {
                1 => Color.white,
                2 => Color.green,
                3 => Color.blue,
                4 => Color.magenta,
                5 => Color.red,
                _ => Color.white
            };

            foreach (var mod in chestItem.modifiers)
            {
                // create a new description text
                var description = Instantiate(DescriptionPrefab, DescriptionContainer);
                // Positive Value.
                if (mod.modifierValue > 0)
                {
                    description.text = "+" + mod.modifierValue + " " + mod.statType;
                    // make the text green
                    description.color = Color.green;
                }
                else if (mod.modifierValue < 0)
                {
                    description.text = "-" + mod.modifierValue + " " + mod.statType;
                    // make the text green
                    description.color = Color.red;
                }
                _descriptions.Add(description);
            }
        }

        public void ApplyItem()
        {
            GameManager.instance.ApplyItem(chestItem);
            
        }
    }
}