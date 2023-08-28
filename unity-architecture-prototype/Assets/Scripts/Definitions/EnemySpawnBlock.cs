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
    public int[] eliteSpawnTimings = { 120 };
    public List<GameObject> eliteEnemies = new();
    public EnemySpawnAction bossAction;

    [Header("Modifiers")]
    public float healthMultiplier = 1f;
    public float healthMultiplierTolerance = 0.0f;
    public float damageMultiplier = 1f;
    public float damageMultiplierTolerance = 0.0f;

    public void OnValidate()
    {
        blockTime = blockTime < 2 ? 2 : blockTime;
    }
}