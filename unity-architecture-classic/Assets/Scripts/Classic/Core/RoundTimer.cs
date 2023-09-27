using UnityEngine;

namespace Classic.Core
{
    public class RoundTimer : MonoBehaviour
    {
        [SerializeField] private GameState state;
        
        [field:SerializeField]
        public float roundTime { get; private set; } = 0f;

        private void OnEnable()
        {
            state.onGameStart.AddListener(() => roundTime = 0f);
        }

        private void Update()
        {
            roundTime += GameTime.deltaTime;
        }
    }
}