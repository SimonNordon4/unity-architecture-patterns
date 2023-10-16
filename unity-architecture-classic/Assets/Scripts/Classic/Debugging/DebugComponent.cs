using UnityEngine;

namespace Classic.Debugging
{
    public class DebugComponent : MonoBehaviour
    {
        protected void Print(string message)
        {
            Debug.Log(message,this);
        }
    }
}