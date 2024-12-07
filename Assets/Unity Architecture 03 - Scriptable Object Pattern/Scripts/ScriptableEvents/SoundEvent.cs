using System;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class SoundEvent : ScriptableObject
    {
        public event Action<AudioClip> OnPlaySound;

        public void PlaySound(AudioClip clip)
        {
            OnPlaySound?.Invoke(clip);
        }
    }
}