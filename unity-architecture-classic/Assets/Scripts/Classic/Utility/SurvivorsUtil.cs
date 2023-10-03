using System.Text;
using UnityEngine;

namespace Classic.Utility
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
        
        public static T Find<T>() where T : class
        {
            var type = typeof(T);
            var objects = UnityEngine.Object.FindObjectsByType(typeof(T),FindObjectsSortMode.None);
            if (objects.Length == 0)
            {
                Debug.LogWarning("No object of type " + type + " found in scene.");
                return null;
            }
            if(objects.Length > 1)
                Debug.LogWarning("Multiple objects of type " + type + " found in scene.");
            return objects[0] as T;
        }
    }
}