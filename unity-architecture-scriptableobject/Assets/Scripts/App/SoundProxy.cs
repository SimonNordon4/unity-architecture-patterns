
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class SoundProxy : MonoBehaviour
    {
        private SoundManager _soundManager;
        
        public void Construct(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        public void PlaySound(SoundDefinition soundDefinition)
        {
            _soundManager.PlaySound(soundDefinition);
        }
    }
}