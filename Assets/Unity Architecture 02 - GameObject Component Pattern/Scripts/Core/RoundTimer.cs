using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class RoundTimer : MonoBehaviour
    {
        [field:SerializeField]
        public float CurrentTime {get;private set;} = 0f;
        private bool _isTimerPaused = false;

        private void Update()
        {
            if (_isTimerPaused) return;
            CurrentTime += Time.deltaTime;
        }

        public void Reset()
        {
            CurrentTime = 0f;
        }
    }
}