using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityArchitecture.SpaghettiPattern
{
    [CreateAssetMenu(fileName = "SoundDefinition", menuName = "UnityArchitecture/SpaghettiPattern/SoundDefinition", order = 1)]
    public class SoundDefinition : ScriptableObject
    {
        public AudioClip[] clips;
        public float volume = 1f;
        public Vector2 pitchVariation = Vector2.one;
    }
}