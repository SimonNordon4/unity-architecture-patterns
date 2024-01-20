using System.Collections.Generic;
using System.Linq;
using GameObjectComponent.Items;
using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "WaveDefinition", menuName = "Classic/WaveDefinition")]
    public class WaveDefinition : ScriptableObject
    {
        [field:SerializeField] public int normalEnemies = 100;
        [field:SerializeField] public float waveDuration { get; private set; }= 60f;
        [field:SerializeField] public List<SpawnActionDefinition> spawnActions {get;private set;} = new();
        [field:SerializeField] public List<SpawnActionDefinition> bossActions {get;private set;} = new();
        
        [field:SerializeField] public int idealEnemiesAlive {get;private set;} = 5;
        [field:SerializeField] public int decay {get;private set;} = 5;
        [field:SerializeField] public Vector2 healthMultiplier {get;private set;} = new Vector2(1, 1);
        [field:SerializeField] public Vector2 damageMultiplier {get;private set;} = new Vector2(1, 1);
        
        [field:SerializeField] public ChestType rewardChest {get;private set;} = ChestType.Medium;
        
        public int TotalActorsCount()
        {
            var bossEnemies = bossActions.Sum(action => action.numberOfEnemiesToSpawn);
            return normalEnemies + bossEnemies;
        }
    }
}