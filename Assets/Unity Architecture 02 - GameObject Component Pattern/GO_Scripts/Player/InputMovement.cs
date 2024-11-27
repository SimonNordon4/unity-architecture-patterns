using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameObjectComponent.Player
{
    public class InputMovement : GameplayComponent
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Stats stats;
        
        private Stat _speedStat;
        
        private Vector3 _lastDirection;

        private void OnEnable()
        {
            _lastDirection = Vector3.zero;
        }

        private void OnDisable()
        {
            _lastDirection = Vector3.zero;
        }

        private void Start()
        {
            _speedStat = stats.GetStat(StatType.MoveSpeed);
        }

        private void Update()
        {
            float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
            float moveVertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow
            var direction = new Vector3(moveHorizontal, 0, moveVertical).normalized;
            
            if (direction != _lastDirection)
            {
                _lastDirection = direction;
            }

            movement.SetVelocity(_lastDirection * _speedStat.value);
            movement.SetLookDirection(_lastDirection);
        }
    }
}