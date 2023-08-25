using System.Collections.Generic;
using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(fileName = "EnemySpawnAction", menuName = "Prototype/EnemySpawnAction", order = 1)]
    public class EnemySpawnAction : ScriptableObject
    {
        public GameObject enemyPrefab;
        public int numberOfEnemiesToSpawn;
        public int spawnChance;
    }
}