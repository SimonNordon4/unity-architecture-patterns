using UnityEngine;

namespace GameObjectComponent.Game
{
    /// <summary>
    /// Keeps track of the round time.
    /// </summary>
    public class RoundTimer : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [field:SerializeField] public float roundTime { get; private set; } = 0f;

        private void OnEnable()
        {
            state.OnGameStart += Reset;
        }

        private void OnDisable()
        {
            state.OnGameStart -= Reset;
        }

        private void Update()
        {
            roundTime += GameTime.deltaTime;
        }

        private void Reset()
        {
            roundTime = 0f;
        }
    }
}