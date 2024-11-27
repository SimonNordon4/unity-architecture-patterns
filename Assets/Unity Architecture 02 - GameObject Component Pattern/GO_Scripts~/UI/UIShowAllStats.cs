using System;
using System.Collections.Generic;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIShowAllStats : MonoBehaviour
    {
        [SerializeField] private RectTransform statsContainer;
        [SerializeField] private UIStatDisplay statDisplayPrefab;
        [SerializeField] private Stats stats;
        
        private readonly List<UIStatDisplay> _statDisplays = new();
        
        private void OnEnable()
        {
            DestroyAll();
            
            foreach (var stat in stats.stats)
            {
                var statDisplay = Instantiate(statDisplayPrefab, statsContainer);
                statDisplay.Construct(stats, stat.type);
                statDisplay.Init();
                _statDisplays.Add(statDisplay);
            }
        }

        private void OnDisable()
        {
            DestroyAll();
        }

        private void DestroyAll()
        {
            foreach (var statDisplay in _statDisplays)
            {
                Destroy(statDisplay.gameObject);
            }
            
            _statDisplays.Clear();
        }
    }
}