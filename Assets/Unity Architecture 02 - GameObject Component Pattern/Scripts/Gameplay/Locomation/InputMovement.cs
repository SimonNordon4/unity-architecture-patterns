using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Stats))]
    public class InputMovement : MonoBehaviour
    {
        private Movement _movement;
        private Stats _stats;
        private Stat _speedStat;
        private Vector3 _lastDirection;

        private void Awake()
        {
            _movement = GetComponent<Movement>();
            _stats = GetComponent<Stats>();
        }

 

        private void Start()
        {
            _speedStat = _stats.GetStat(StatType.Speed);
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

            _movement.SetVelocity(_lastDirection * _speedStat.value);
            _movement.SetLookDirection(_lastDirection);
        }
    }
}