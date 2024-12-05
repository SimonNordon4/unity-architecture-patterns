using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] private float defaultTimeScale = 1f;
        
        public void PauseTime()
        {
            Time.timeScale = 0;
        }

        public void ResumeTime()
        {
            Time.timeScale = defaultTimeScale;
        }
    }
}