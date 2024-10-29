using System.Globalization;
using UnityEngine;

namespace UnityArchitecture.SharedAssets
{
    public class TestDebugger : MonoBehaviour
    {
        private float value;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Console.Log("TestDebugger.Start()", LogFilter.Account);
            Console.Log("TestDebugger.Start()", LogFilter.Game);
            Console.Log("TestDebugger.Start()", LogFilter.Player);
            Console.Log("TestDebugger.Start()", LogFilter.Chest);
            Console.Log("TestDebugger.Start()", LogFilter.Enemy);
            Console.Log("TestDebugger.Start()", LogFilter.UI);
            Debug.Log("TestDebugger.Start()");
        }

        // Update is called once per frame
        void Update()
        {
            value = Mathf.Sin(Time.time);
            Console.Watch($"Value:", value.ToString(CultureInfo.InvariantCulture));
        }
    }
}

