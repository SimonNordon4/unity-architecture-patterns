using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class SoundEvent : ScriptableObject
    {
        public UnityEvent<AudioClip> OnPlaySound = new();

        public void PlaySound(AudioClip clip)
        {
            OnPlaySound?.Invoke(clip);
        }
    }
}