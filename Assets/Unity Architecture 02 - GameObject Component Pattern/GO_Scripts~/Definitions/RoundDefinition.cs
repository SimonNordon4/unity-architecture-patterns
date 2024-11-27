using System.Collections.Generic;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "RoundDefinition", menuName = "Classic/RoundDefinition")]
    public class RoundDefinition : ScriptableObject
    {
        [field:SerializeField] public List<WaveDefinition> waves { get; private set; }
    }
}