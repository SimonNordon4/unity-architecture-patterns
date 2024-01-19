using GameplayComponents;
using GameplayComponents.Locomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameObjectComponent.Player
{
    public class InputDash : GameplayComponent
    {
        [SerializeField] private Dash dash;
        [SerializeField] private InputActionAsset actionMap;
        private InputAction _dashAction;
        
        private void OnEnable()
        {
            _dashAction = actionMap.FindAction("Dash");
            _dashAction.performed += OnDashPerformed;
            _dashAction.Enable();
        }
        
        private void OnDisable()
        {
            _dashAction.performed -= OnDashPerformed;
            _dashAction.Disable();
        }

        private void OnDashPerformed(InputAction.CallbackContext obj)
        {
            dash.DashForward();
        }
    }
}