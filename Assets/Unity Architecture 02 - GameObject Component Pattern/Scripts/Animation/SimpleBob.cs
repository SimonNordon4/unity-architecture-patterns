using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class SimpleBob : MonoBehaviour
    {
        public float bobHeight = 0.5f;     // The maximum height difference for the bobbing motion.
        public float bobSpeed = 1.0f;      // Speed of the bobbing motion.

        private float initialY;             // Y position at the start.
        private float time;

        // Start is called before the first frame update
        void Start()
        {
            // Capture the starting position.
            initialY = transform.position.y;
            time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime * bobSpeed;

            // Calculate the new Y position based on the sine wave.
            float newY = initialY + Mathf.Sin(time) * bobHeight;

            // Update the position.
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}