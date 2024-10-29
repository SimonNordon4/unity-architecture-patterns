using UnityEngine;

public static class Console
{
    public static void Log(string message = "", LogFilter logFilter = LogFilter.None, UnityEngine.Object inContext = null)
    {
        #if UNITY_ARCHITECTURE_DEBUG
        Debug.Log(message + "\nCPAPI:{\"cmd\":\"Filter\", \"name\":\"" + logFilter.ToString() + "\"}", inContext);
        #endif
    }

    public static void LogWarning(string message = "", UnityEngine.Object inContext = null)
    {
        #if UNITY_ARCHITECTURE_DEBUG
        Debug.LogWarning(message, inContext);
        #endif
    }

    public static void LogError(string message = "", UnityEngine.Object inContext = null)
    {
        #if UNITY_ARCHITECTURE_DEBUG
        Debug.LogError(message, inContext);
        #endif
    }

    public static void Watch(string label, string value)
    {
        #if UNITY_ARCHITECTURE_DEBUG
        Debug.Log(label + " : " + value + "\nCPAPI:{\"cmd\":\"Watch\", \"name\":\"" + label + "\"}");
        #endif
    }
}

public enum LogFilter
{
    None,
    Account,
    Game,
    Player,
    Chest,
    Enemy,
    UI
}
