using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class SimpleRotate : MonoBehaviour
    {
        // Create a public float variable to control the rotation speed from the Unity Editor
        public float rotationSpeed = 30f; // Rotating at 30 degrees per second as default.

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Rotate the GameObject on its Y-axis.
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}