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
    }
}