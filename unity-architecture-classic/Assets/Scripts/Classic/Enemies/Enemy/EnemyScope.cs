using Classic.Game;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    /// <summary>
    /// The purpose of the enemy scope is to give the Enemy Object access to Scene References.
    /// This all scenes references should be injected into the scope.
    /// </summary>
    public class EnemyScope : MonoBehaviour
    {

        [field: SerializeField] public EnemyType type { get; private set; }
        [field: SerializeField] public GameState state { get; private set; }
        [field: SerializeField] public Level level { get; private set; }
        [field: SerializeField] public EnemyEvents events { get; private set; }
        [field: SerializeField] public Transform characterTransform { get; private set; }

        public void Construct(
            GameState newState, 
            Level newLevel,  
            EnemyEvents newEvents,
            Transform newCharacterTransform)
        {
            state = newState;
            level = newLevel;
            events = newEvents;
            characterTransform = newCharacterTransform;
        }
    }
}