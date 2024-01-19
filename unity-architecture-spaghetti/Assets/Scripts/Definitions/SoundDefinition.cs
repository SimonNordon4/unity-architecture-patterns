using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDefinition", menuName = "Prototype/SoundDefinition", order = 1)]
public class SoundDefinition : ScriptableObject
{
    public AudioClip[] clips;
    public float volume = 1f;
    public Vector2 pitchVariation = Vector2.one;
}
