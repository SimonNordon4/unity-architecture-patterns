using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class SoundProxy : MonoBehaviour
    {
        private SoundManager _soundManager;
        
        public void Construct(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        public void PlaySound(AudioClip clip)
        {
            _soundManager.PlaySound(clip);
        }
    }
}