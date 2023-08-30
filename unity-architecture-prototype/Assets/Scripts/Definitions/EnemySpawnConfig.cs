using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Prototype/EnemySpawnConfig", order = 1)]
public class EnemySpawnConfig : ScriptableObject
{
    public List<EnemySpawnBlock> enemySpawnBlocks = new();


}
