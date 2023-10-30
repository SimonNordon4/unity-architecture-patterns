using System.Collections.Generic;
using UnityEngine;

namespace Classic.Enemies
{
    [CreateAssetMenu(fileName = "EnemyRoundDefinition", menuName = "Classic/EnemyRoundDefinition")]
    public class EnemyRoundDefinition : ScriptableObject
    {
        [field:SerializeField] public List<WaveDefinition> waves { get; private set; }
    }
}