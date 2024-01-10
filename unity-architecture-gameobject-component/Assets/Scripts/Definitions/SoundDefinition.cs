using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "SoundDefinition", menuName = "GameObjectComponent/SoundDefinition", order = 1)]
    public class SoundDefinition : ScriptableObject
    {
        public AudioClip[] clips;
        public float volume = 1f;
        public Vector2 pitchVariation = new Vector2(0.9f,1.1f);
    }
}