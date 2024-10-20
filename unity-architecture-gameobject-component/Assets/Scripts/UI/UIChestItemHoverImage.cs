﻿using System.Collections.Generic;
using GameObjectComponent.Definitions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIChestItemHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ChestItemDefinition _item;
        private int _itemCount;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject hoverInfoBox;
        [SerializeField] private TextMeshProUGUI hoverInfoTitle;
        [SerializeField] private TextMeshProUGUI descriptionPrefab;
        [SerializeField] private List<TextMeshProUGUI> hoverInfoStats = new();

        public void Construct(ChestItemDefinition item, int count)
        {
            _item = item;
            _itemCount = count;
        }

        public void Initialize()
        {
            foreach (var infoStat in hoverInfoStats)
            {
                Destroy(infoStat.gameObject);
            }

            hoverInfoStats.Clear();

            // Rebuild the hover info box.
            itemCountText.text = _itemCount + "x";
            hoverInfoTitle.text = _item.itemName;
            icon.sprite = _item.sprite;

            // if _item.sprite is null, make the image dark slate grey
            icon.color = _item.sprite == null ? new Color(0.25f, 0.25f, 0.3f) : Color.white;

            foreach (var mod in _item.modifiers)
            {
                var newDescription = Instantiate(descriptionPrefab, hoverInfoBox.transform);
                newDescription.fontSize = 18;

                var statSign = mod.modifierValue > 0 ? "+" : "-";

                // Format stat value.
                var statValueString = mod.modifierType != ModifierType.Percentage
                    ? statSign + (mod.modifierValue * _itemCount)
                    : $"{statSign}{mod.modifierValue * _itemCount * 100}%";


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

                newDescription.text = statValueString + " " + statTypeString;
                // make the text green
                newDescription.color = mod.modifierValue > 0 ? new Color(0.75f, 1, 0.75f) : new Color(1, 0.75f, 0.75f);

                hoverInfoStats.Add(newDescription);
            }

            hoverInfoBox.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverInfoBox.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverInfoBox.SetActive(false);
        }
    }
}