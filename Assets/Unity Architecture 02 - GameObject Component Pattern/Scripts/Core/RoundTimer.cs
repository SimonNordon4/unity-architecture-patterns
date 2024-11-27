using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class RoundTimer : MonoBehaviour
    {
        [field:SerializeField]
        public float RoundTime {get;private set;} = 0f;
        private bool _isTimerPaused = false;

        private void Update()
        {
            if (_isTimerPaused) return;
            RoundTime += Time.deltaTime;
        }

        public void Reset()
        {
            RoundTime = 0f;
        }
    }
}