using System;
using System.Collections.Generic;
using Classic.Game;
using Classic.Utility;
using TMPro;
using UnityEngine;

namespace Classic.UI
{
    /// <summary>
    /// Initialize and display all stats in the stats map
    /// </summary>
    public class UIAllStats : MonoBehaviour
    {
        [SerializeField] private Stats stats;
        [SerializeField] private UIStatDescription statDescriptionPrefab;
        [SerializeField] private RectTransform statContainer;
        private Dictionary<StatType, UIStatDescription> _statTextMap = new();
        
        private bool _isInitialized = false;
        
        private void Start()
        {
            PopulateStatText();
            UpdateStats();
        }

        private void OnEnable()
        {
            if(!_isInitialized) PopulateStatText();
            UpdateStats();
        }

        private void PopulateStatText()
        {
            // instantitate a stat text for each stat type
            foreach (var stat in stats.statMap.Values)
            {
                if(_statTextMap.ContainsKey(stat.type)) continue;
                var statType = stat.type;
                var statDescription = Instantiate(statDescriptionPrefab, statContainer);
                statDescription.SetStatDescription(stat);
                _statTextMap.Add(statType, statDescription);
            }
            
            _isInitialized = true;
        }

        private void UpdateStats()
        {
            // update the value of each stat type
            foreach (var statType in stats.statMap.Keys)
            {
                var text = _statTextMap[statType];
                var stat = stats.statMap[statType];
                text.SetStatDescription(stat);
            }
        }
    }
}