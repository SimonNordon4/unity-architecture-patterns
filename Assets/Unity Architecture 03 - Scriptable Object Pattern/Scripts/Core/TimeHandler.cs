using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class TimeHandler : ScriptableData
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

        public override void ResetData()
        {
            Time.timeScale = defaultTimeScale;
        }
    }
}