using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemySpawnBlock", menuName = "Prototype/EnemySpawnBlock", order = 1)]
public class EnemySpawnBlock : ScriptableObject
{
    public Vector2 healthMultiplier = new Vector2(1, 1);
    public Vector2 damageMultiplier = new Vector2(1, 1);
    public Vector2Int bossChestTier = new Vector2Int(1, 2);
    public Vector2Int bossChestChoices = new Vector2Int(3, 3);
    
    public List<EnemySpawnWave> spawnWaves = new();
}
