using System.Collections.Generic;
using GameObjectComponent.App;
using TMPro;
using UnityEngine;

public class UIStatisticsMenu : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StatisticsManager statisticsManager;
    
    [Header("UI")] 
    [SerializeField]private Color statisticColor;
    [SerializeField]private Color labelColor;

    [SerializeField] private RectTransform leftColumn;
    [SerializeField] private RectTransform rightColumn;

    [SerializeField] private TextMeshProUGUI statisticTextPrefab;

    private bool _isInitialized;
    private Dictionary<Statistic, TextMeshProUGUI> _textMap = new();
    
    private void CreateUI()
    {
        // Get all statistics
        var statistics = statisticsManager.statistics;
        if (statistics == null || statistics.Length == 0) return;

        for(var i = 0; i < statistics.Length; i++)
        {
            var column = i < statistics.Length / 2 ? leftColumn : rightColumn;
            var newText = Instantiate(statisticTextPrefab, column);
            _textMap.Add(statistics[i], newText);
        }
        _isInitialized = true; // Set the initialization flag to true
    }

    public void UpdateUI()
    {
        if(!_isInitialized) CreateUI();
        
        var label = ColorUtility.ToHtmlStringRGB(labelColor);
        var stat = ColorUtility.ToHtmlStringRGB(statisticColor);
        
        foreach(var pair in _textMap)
        {
            var statistic = pair.Key;
            var text = pair.Value;
            text.text = $"<color=#{label}>{statistic.name}:</color> <color=#{stat}>{statistic.highestValue}</color>";
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }
}