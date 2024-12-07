using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class RoundTimer : MonoBehaviour
    {
        private static RoundTimer _instance;
        public static RoundTimer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<RoundTimer>();
                }
                return _instance;
            }
        }

        [field:SerializeField]
        public float CurrentTime {get;private set;} = 0f;
        private bool _isTimerPaused = false;

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (_isTimerPaused) return;
            CurrentTime += Time.deltaTime;
        }

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
    }
}