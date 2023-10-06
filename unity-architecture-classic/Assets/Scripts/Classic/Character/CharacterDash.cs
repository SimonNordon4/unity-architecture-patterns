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
        [SerializeField] private Level level;

        private Vector3 _dashDestination;
        [SerializeField] private float dashDistance = 1f;
        [SerializeField] private float dashTime = 0.5f;

        private bool _isDashing;
        private float _timeSinceDashStarted = 0f;

        private void Update()
        {
            if (!state.isGameActive) return;

            if (Input.GetKeyDown(dashKey) && !_isDashing)
            {
                if (stats.dashes.value <= 0) return;
                _isDashing = true;
                movement.enabled = false;
                _dashDestination = transform.position + movement.direction * dashDistance;
                _timeSinceDashStarted = 0f;
                stats.dashes.value--;
            }

            if (_isDashing)
            {
                var dashProgress = _timeSinceDashStarted / dashTime;
                _timeSinceDashStarted += GameTime.deltaTime;
                
                if (_timeSinceDashStarted < dashTime)
                {
                    var position = transform.position;
                    position = Vector3.Lerp(position, _dashDestination, dashProgress);
                    
                    // clamp to level bounds
                    var clampedPosition = position;
                    clampedPosition.x = Mathf.Clamp(clampedPosition.x, -level.bounds.x, level.bounds.x);
                    clampedPosition.z = Mathf.Clamp(clampedPosition.z, -level.bounds.y, level.bounds.y);
                    position = clampedPosition;
                    transform.position = position;
                }
                else
                {
                    // Ensure we reach the exact destination and stop dashing
                    transform.position = _dashDestination;
                    _isDashing = false;
                    movement.enabled = true;
                }
            }
        }
    }
}