using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityArchitecture.SpaghettiPattern
{
    [CreateAssetMenu(fileName = "EnemySpawnRound", menuName = "UnityArchitecture/SpaghettiPattern/EnemySpawnConfig", order = 1)]
    public class EnemySpawnRound : ScriptableObject
    {
        [Inline]
        public List<EnemySpawnBlock> enemySpawnBlocks = new();
    }
}