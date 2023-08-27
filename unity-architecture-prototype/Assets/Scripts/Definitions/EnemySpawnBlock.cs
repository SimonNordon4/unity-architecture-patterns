using System.Collections.Generic;
using Definitions;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnBlock", menuName = "Prototype/EnemySpawnBlock", order = 1)]
public class EnemySpawnBlock : ScriptableObject
{
    public EnemyManager enemyManager;
    public int totalEnemies = 100;
    public AnimationCurve spawnRateCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));

    public List<EnemySpawnAction> enemySpawnActions = new();
    public int[] eliteSpawnTimings = { 120 };
    public List<GameObject> eliteEnemies = new();
    public EnemySpawnAction bossAction;

    public float blockTime = 300f;

    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
    }
}