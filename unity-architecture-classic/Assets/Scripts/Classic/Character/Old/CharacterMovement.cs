using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private Transform characterTransform;
        [SerializeField] private GameState state;
        [SerializeField] private Stats stats;
        [SerializeField] private Level level;
        public Vector3 direction { get; set; } = Vector3.zero;
        private Transform _transform;
        private Vector3 _initialOffset;

        private void Awake()
        {
            _initialOffset = characterTransform.position;
        }

        private void OnEnable()
        {
            state.OnGameStart+=(Reset);
        }

        private void OnDisable()
        {
            state.OnGameStart-=(Reset);
        }

        private void LateUpdate()
        {
             if (!state.isGameActive) return;

             var dir = direction;
             
             // Check if the player is at the level bounds, if they are, make sure they cant move in the direction of the bound
             if (characterTransform.position.x <= -level.bounds.x && dir.x < 0)
                 dir.x = 0;
             if (characterTransform.position.x >= level.bounds.x && dir.x > 0)
                 dir.x = 0;
             if (characterTransform.position.z <= -level.bounds.y && dir.z < 0)
                 dir.z = 0;
             if (characterTransform.position.z >= level.bounds.y && dir.z > 0)
                 dir.z = 0;
             
             if(dir.magnitude < 0.01f) return;
             
             var speed = stats.playerSpeed.value;
             var velocity = dir * (speed * GameTime.deltaTime);

             characterTransform.position += velocity;
             characterTransform.rotation = Quaternion.LookRotation(velocity);
        }
        public void Reset()
        {
            characterTransform.position = _initialOffset;
        }
    }
}