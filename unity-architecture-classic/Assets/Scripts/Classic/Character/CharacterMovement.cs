using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private Level level;
        public Vector3 direction { get; set; } = Vector3.zero;
        private Transform _transform;
        private Vector3 _initialOffset;

        private void Awake()
        {
            _initialOffset = transform.position;
        }

        private void OnEnable()
        {
            state.onGameStart.AddListener(Reset);
            _transform = transform;
        }

        private void OnDisable()
        {
            state.onGameStart.RemoveListener(Reset);
        }

        private void LateUpdate()
        {
             if (!state.isGameActive) return;

             var dir = direction;
             
             // Check if the player is at the level bounds, if they are, make sure they cant move in the direction of the bound
             if (_transform.position.x <= -level.bounds.x && dir.x < 0)
                 dir.x = 0;
             if (_transform.position.x >= level.bounds.x && dir.x > 0)
                 dir.x = 0;
             if (_transform.position.z <= -level.bounds.y && dir.z < 0)
                 dir.z = 0;
             if (_transform.position.z >= level.bounds.y && dir.z > 0)
                 dir.z = 0;
             
             if(dir.magnitude < 0.01f) return;
             
             var speed = stats.playerSpeed.value;
             var velocity = dir * (speed * GameTime.deltaTime);

             _transform.position += velocity;
             _transform.rotation = Quaternion.LookRotation(velocity);
        }
        private void Reset()
        {
            transform.position = _initialOffset;
        }
    }
}