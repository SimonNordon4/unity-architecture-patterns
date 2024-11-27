using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameObjectComponent.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{


    public class UIChestItemButton : MonoBehaviour
    {
        public ChestItemDefinition chestChestItem;
        public TextMeshProUGUI title;
        public TextMeshProUGUI descriptionPrefab;
        public RectTransform descriptionContainer;
        public Image borderImage;
        public Sprite[] borderTiers;
        public Image itemSprite;

        private List<TextMeshProUGUI> _descriptions = new List<TextMeshProUGUI>();

        public void Initialize(ChestItemDefinition chestItem)
        {
            if (chestItem == null)
            {
                Debug.LogError("Item is null",this);
            }
            foreach (var description in _descriptions)
            {
                Destroy(description.gameObject);
            }

            _descriptions.Clear();

            title.text = chestItem.itemName;
            chestChestItem = chestItem;

            itemSprite.sprite = chestItem.sprite;

            borderImage.sprite = borderTiers[chestChestItem.tier - 1];

            foreach (var mod in chestChestItem.modifiers)
            {
                // create a new description text
                var description = Instantiate(descriptionPrefab, descriptionContainer);

                // we don't need a minus because negative values will already have a minus
                var statSign = mod.modifierValue > 0 ? "+" : "";

                // Format stat value.
                var statValueString = mod.modifierType != ModifierType.Percentage
                    ? statSign + mod.modifierValue
                    : $"{statSign}{mod.modifierValue * 100}%";


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
                description.color = mod.modifierValue > 0 ? new Color(0.75f, 1, 0.75f) : new Color(1, 0.75f, 0.75f);

                _descriptions.Add(description);
            }
        }
    }
}