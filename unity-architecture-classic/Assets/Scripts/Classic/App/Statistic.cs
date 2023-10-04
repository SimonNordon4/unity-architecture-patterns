using System;

namespace Classic.App
{
    [Serializable]
    public class Statistic
    {
        public Statistic()
        {
            
        }

        public Statistic(StatisticDefinition def)
        {
            name = def.name;
            statisticType = def.statisticType;
            statType = def.statType;
        }
        
        public string name;
        public StatisticType statisticType;
        public int highestValue;
        public StatType statType;
    }
}