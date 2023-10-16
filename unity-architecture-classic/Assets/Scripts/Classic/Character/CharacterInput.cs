using Classic.Actor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Classic.Character
{
    public class CharacterInput : ActorComponent
    {
        [SerializeField] private InputActionAsset actionAsset;
        [SerializeField] private ActorMovement movement;
        [SerializeField] private ActorStats stats;
        private Stat _speedStat;
        private InputAction _movementAction;

        private void Awake()
        {
            var map = actionAsset.FindActionMap("Player");
            _movementAction = map.FindAction("Direction");
        }

        private void Start()
        {
            _speedStat = stats.Map[StatType.MoveSpeed];
        }

        private void OnEnable()
        {
            _movementAction.Enable();
        }

        private void OnDisable()
        {
            _movementAction.Disable();
        }

        private void Update()
        {
            var movementInput = _movementAction.ReadValue<Vector2>();
            var direction = new Vector3(movementInput.x, 0, movementInput.y);
            movement.velocity = direction * _speedStat.value;
            movement.lookDirection = direction;
        }
    }
}