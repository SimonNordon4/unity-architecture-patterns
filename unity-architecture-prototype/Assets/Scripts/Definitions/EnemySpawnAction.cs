using System;
using System.Collections.Generic;
using UnityEngine;

namespace Definitions
{
    [Serializable]
    public class EnemySpawnAction
    {
        public GameObject enemyPrefab;
        public int health = 5;
        public int damage = 1;
        public int numberOfEnemiesToSpawn = 1;
        public int spawnChance = 100;
    }
}