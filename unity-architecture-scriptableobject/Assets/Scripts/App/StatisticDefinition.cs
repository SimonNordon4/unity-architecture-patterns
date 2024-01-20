using UnityEngine;

namespace GameObjectComponent.App
{
    [CreateAssetMenu(fileName = "StatisticDefinition", menuName = "Classic/StatisticDefinition")]
    public class StatisticDefinition : ScriptableObject
    {
        public string name;
        public StatisticType statisticType;
        public StatType statType;
    }
}