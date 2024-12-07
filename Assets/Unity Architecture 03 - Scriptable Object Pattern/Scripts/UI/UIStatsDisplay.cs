using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIStatsDisplay : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Stats playerStats;

        [Header("Stats UI")] [SerializeField] private RectTransform statContainer;
        [SerializeField] private TextMeshProUGUI textDescriptionPrefab;
        [SerializeField] private Color defaultColor = Color.gray;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;


        private List<TextMeshProUGUI> textDescriptions = new();

        private void OnEnable()
        {
            ClearStatsDisplay();
            PopulateStats();
        }

        private void OnDisable()
        {
            ClearStatsDisplay();
        }

        private void ClearStatsDisplay()
        {
            foreach (TextMeshProUGUI textDescription in textDescriptions)
            {
                Destroy(textDescription.gameObject);
            }

            textDescriptions.Clear();
        }

        private void PopulateStats()
        {
            if (playerStats == null)
            {
                Debug.LogWarning("PlayerStats reference is not assigned.");
                return;
            }

            foreach (Stat stat in playerStats.StatsList)
            {
                var statText = Instantiate(textDescriptionPrefab, statContainer);

                Color colorToUse;
                if (stat.value > stat.baseValue) colorToUse = positiveColor;
                else if (stat.value < stat.baseValue) colorToUse = negativeColor;
                else colorToUse = defaultColor;

                var colorHex = ColorUtility.ToHtmlStringRGB(colorToUse);
                statText.text = $"{stat.StatType}: <color=#{colorHex}>{stat.value}</color>";

                textDescriptions.Add(statText);
            }
        }
    }
}