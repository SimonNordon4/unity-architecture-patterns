using Classic.Utility;
using TMPro;
using UnityEngine;

namespace Classic.UI
{
    public class UIStatDescription : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statName;
        [SerializeField] private TextMeshProUGUI statValue;
        
        [SerializeField] private Color defaultColor = new Color(0.7f,0.7f,0.7f);
        [SerializeField] private Color positiveColor = new Color(0.66f,1f,0.66f);
        [SerializeField] private Color negativeColor = new Color(1f,0.5f,0.5f);
        
        public void SetStatDescription(Stat stat)
        {
            statName.text = SurvivorsUtil.CamelCaseToString(stat.type.ToString());
            statValue.text = stat.value.ToString("F0");
            statValue.color = stat.value > stat.initialValue ? positiveColor : stat.value < stat.initialValue ? negativeColor : defaultColor;
        }
    }
}