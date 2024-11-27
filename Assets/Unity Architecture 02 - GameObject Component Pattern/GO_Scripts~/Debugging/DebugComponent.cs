using UnityEngine;

namespace GameObjectComponent.Debugging
{
    public class DebugComponent : MonoBehaviour
    {
        protected void Print(string message)
        {
            Debug.Log(message,this);
        }
    }
}