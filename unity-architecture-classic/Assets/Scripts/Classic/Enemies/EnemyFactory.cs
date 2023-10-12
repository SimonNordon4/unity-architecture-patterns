using System;
using System.Collections.Generic;
using Classic.Enemies.Enemy;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [SerializeField] private EnemyEvents events;
        [SerializeField] private Transform characterTransform;


        public EnemyScope Create(EnemyDefinition enemyDefinition, Vector3 position = new())
        {
            var enemyScope = Instantiate(enemyDefinition.enemyPrefab, position, Quaternion.identity, null);
            enemyScope.Construct(state, level,events, characterTransform);
            
            if(enemyScope.TryGetComponent<EnemyStats>(out var enemyStats))
            {
                enemyStats.Initialize(enemyDefinition);
            }
            
            if (enemyScope.type != enemyDefinition.enemyType)
            {
                Debug.LogError("EnemyScope type does not match EnemyDefinition type");
            }
            return enemyScope;
        }
    }
}