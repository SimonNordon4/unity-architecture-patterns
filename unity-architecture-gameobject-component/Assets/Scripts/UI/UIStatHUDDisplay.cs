using GameObjectComponent.Game;
using GameObjectComponent.Utility;
using GameplayComponents.Actor;
using TMPro;
using UnityEngine;
namespace GameObjectComponent.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIStatDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField]private StatType statType;
        [SerializeField]private Stats stats;
        [SerializeField]private Color positiveColor = new Color(0.66f,1f,0.66f);
        [SerializeField]private Color negativeColor = new Color(1f,0.5f,0.5f);
        private Stat _stat;
    
        private void OnEnable()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _stat = stats.GetStat(statType);
            _stat.onStatChanged += OnStatChanged;
        }

        private void OnStatChanged()
        {
            var statName = SurvivorsUtil.CamelCaseToString(statType.ToString());
            var color = _stat.value > 0 ? positiveColor: negativeColor;
            var htmlColor = ColorUtility.ToHtmlStringRGB(color);
            _textMeshProUGUI.text = $"{statName}: <color=#{htmlColor}>{_stat.value:F0}</color>";
        }
    }
}