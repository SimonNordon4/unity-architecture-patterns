using System.Collections.Generic;
using UnityArchitecture.GameObjectComponentPattern;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDirectorWave", menuName = "GameObjectComponent/EnemyDirectorWave", order = 1)]
public class EnemyDirectorWave : ScriptableObject
{
    public int breakPoint = 0;
    public List<EnemyType> enemyTypes = new();
    public List<EnemyType> bossTypes = new();
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public int maxEnemiesAlive = 3;
}