using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyScope : MonoBehaviour
    {
        [field: SerializeField] public GameState gameState { get; private set; }
        [field: SerializeField] public Level level { get; private set; }
        [field: SerializeField] public EnemyEvents enemyEvents { get; private set; }
        [field: SerializeField] public EnemyDefinition definition { get; private set; }
        [field: SerializeField] public bool isBoss { get; private set; }

        public void Construct(GameState state, Level lvl, EnemyEvents events, bool boss = false)
        {
            gameState = state;
            level = lvl;
            enemyEvents = events;
            isBoss = boss;
        }
    }
}