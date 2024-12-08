using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class RoundTimer : ScriptableBehaviour
    {
        [field:SerializeField]
        public float CurrentTime {get;private set;} = 0f;
        private bool _isTimerPaused = false;

        public void Reset()
        {
            CurrentTime = 0f;
        }

        public void Pause()
        {
            _isTimerPaused = true;
        }

        public void Resume()
        {
            _isTimerPaused = false;
        }

        public override void ResetData()
        {
            _isTimerPaused = false;
            CurrentTime = 0f;
        }

        public override void OnUpdate()
        {
            if (_isTimerPaused) return;
            CurrentTime += Time.deltaTime;
        }
    }
}