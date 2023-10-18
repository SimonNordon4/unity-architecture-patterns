using Classic.Actors;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [SerializeField] private Transform initialTarget;

        public GameObject Create(EnemyDefinition enemyDefinition, Vector3 position = new())
        {
            var enemy = Instantiate(enemyDefinition.enemyPrefab, position, Quaternion.identity, null);
            
            if(enemy.TryGetComponent<ActorGameState>(out var gameState))
                gameState.Construct(state);
            
            if(enemy.TryGetComponent<ActorMovement>(out var movement))
                movement.Construct(level);
            
            if(enemy.TryGetComponent<ActorTarget>(out var target))
                target.SetTarget(initialTarget);
            
            return enemy;
        }
    }
}