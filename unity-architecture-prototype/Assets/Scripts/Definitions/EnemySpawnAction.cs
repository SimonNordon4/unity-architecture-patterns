using System;
using System.Collections.Generic;
using UnityEngine;

namespace Definitions
{
    [Serializable]
    public class EnemySpawnAction
    {
        public GameObject enemyPrefab;
        public int numberOfEnemiesToSpawn = 1;
        public int spawnWeight = 100;
    }
}