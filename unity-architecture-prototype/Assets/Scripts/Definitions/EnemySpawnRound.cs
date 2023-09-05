using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnRound", menuName = "Prototype/EnemySpawnConfig", order = 1)]
public class EnemySpawnRound : ScriptableObject
{
    public int[] tierBreaks;
    public List<EnemySpawnBlock> enemySpawnBlocks = new();
}
