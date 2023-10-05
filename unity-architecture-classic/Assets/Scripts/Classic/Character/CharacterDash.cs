using Classic.Game;
using UnityEngine;

namespace Classic.Character
{
    public class CharacterDash : MonoBehaviour
    {
        [SerializeField] private Stats stats;
        [SerializeField] private GameState state;
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private KeyCode dashKey = KeyCode.Space;

        private Vector3 _dashDestination;
        private float dashSpeed;

        private bool _isDashing;
        private void Update()
        {
            if (!state.isGameActive) return;
            if (stats.revives.value <= 0) return;
            if (Input.GetKeyDown(dashKey))
            {
                _isDashing = true;
                movement.enabled = false;
                _dashDestination = movement.direction * dashSpeed;
            }

            if (_isDashing)
            {
                // inverse quadratic to the dash destination.
            }
        }
    }
}