using System.Collections.Generic;
using Definitions;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnBlock", menuName = "Prototype/EnemySpawnBlock", order = 1)]
public class EnemySpawnBlock : ScriptableObject
{
    public EnemyManager enemyManager;
    public List<EnemySpawnAction> enemySpawnActions = new();

    [Header("Base Stats")]
    public int totalEnemies = 100;
    public float blockTime = 300f;
    public AnimationCurve spawnRateCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public EnemySpawnAction bossAction;
    public int bossChestTier = 1;
    public int bossChestItems = 1;

    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
    }
}