using System.Text;
using UnityEngine;

namespace GameObjectComponent.Utility
{
    public static class SurvivorsUtil
    {
        public static string CamelCaseToString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            var result = new StringBuilder();

            // Capitalize the first letter
            result.Append(char.ToUpper(str[0]));

            for (int i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    result.Append(' ');
                }
                result.Append(str[i]);
            }

            return result.ToString();
        }
        
        public static string FormatModifierValue(Modifier mod)
        {
            var statSign = mod.modifierValue > 0 ? "+" : "-";                

            // Format stat value.
            var statValueString = mod.modifierType != ModifierType.Percentage ?
                statSign + (mod.modifierValue) :
                $"{statSign}{mod.modifierValue * 100}%";

                    
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
            
            return statValueString + " " + statTypeString;
        }
    }
}