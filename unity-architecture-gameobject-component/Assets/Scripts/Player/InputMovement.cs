using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Locomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameObjectComponent.Player
{
    public class InputMovement : GameplayComponent
    {
        [SerializeField] private Movement movement;
        [SerializeField] private InputActionAsset actionMap;
        [SerializeField] private Stats stats;
        
        private Stat _speedStat;
        private InputAction _movementAction;
        private Vector3 _lastDirection;

        private void OnEnable()
        {
            _lastDirection = Vector3.zero;
            _movementAction = actionMap.FindAction("Direction");
            _movementAction.performed += OnMovementPerformed;
            _movementAction.canceled += OnMovementCanceled;
            _movementAction.Enable();
        }

        private void OnDisable()
        {
            _lastDirection = Vector3.zero;
            _movementAction.performed -= OnMovementPerformed;
            _movementAction.canceled -= OnMovementCanceled;
            _movementAction.Disable();
        }

        private void Start()
        {
            _speedStat = stats.GetStat(StatType.MoveSpeed);
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            var movementInput = context.ReadValue<Vector2>();
            var direction = new Vector3(movementInput.x, 0, movementInput.y);
            if (direction != _lastDirection)
            {
                _lastDirection = direction;
            }
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _lastDirection = Vector3.zero;
        }

        private void Update()
        {
            movement.SetVelocity(_lastDirection * _speedStat.value);
            movement.SetLookDirection(_lastDirection);
        }
    }
}