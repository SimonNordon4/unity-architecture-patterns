using System;
using System.Collections.Generic;
using Classic.Enemies;
using UnityEngine;

namespace Definitions
{
    [Serializable]
    public class EnemySpawnAction
    {
        public GameObject enemyPrefab;
        [field: SerializeField] public EnemyDefinition definition { get; private set; }
        public int numberOfEnemiesToSpawn = 1;
        public int spawnWeight = 100;
        
        public int GetHealth()
        {
            return enemyPrefab.GetComponent<EnemyController>().currentHealth;
        }

        public int GetDamage()
        {
            return enemyPrefab.GetComponent<EnemyController>().damageAmount;
        }
    }
}