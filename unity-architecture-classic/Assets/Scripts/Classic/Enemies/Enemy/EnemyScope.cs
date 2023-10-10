using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Enemies.Enemy
{
    public class EnemyScope : MonoBehaviour
    {
        [field:Header("Scene Dependencies")]
        [field: SerializeField] public GameState gameState { get; private set; }
        [field: SerializeField] public Level level { get; private set; }
        [field: SerializeField] public EnemyEvents enemyEvents { get; private set; }
        [field: SerializeField] public EnemyDefinition definition { get; private set; }
        [field: SerializeField] public EnemyPool enemyPool { get; private set; }
        [field: SerializeField] public bool isBoss { get; private set; }
        
        public void Construct(GameState state, Level lvl, EnemyPool pool, EnemyEvents events, bool boss = false)
        {
            gameState = state;
            level = lvl;
            enemyPool = pool;
            enemyEvents = events;
            isBoss = boss;
        }

        public void Destroy()
        {
            if (enemyPool != null)
            {
                enemyPool.DeSpawn(this);
                return;
            }
            Destroy(gameObject);
        }
    }
}