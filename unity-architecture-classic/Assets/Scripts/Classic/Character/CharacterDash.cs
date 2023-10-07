using Classic.Game;
using Classic.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Character
{
    public class CharacterDash : MonoBehaviour, IResettable
    {
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Stats stats;
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private KeyCode dashKey = KeyCode.Space;
        [SerializeField] private Level level;

        private Vector3 _dashDestination;
        [SerializeField] private float dashDistance = 1f;
        [SerializeField] private float dashTime = 0.5f;
        
        public UnityEvent onDash = new();

        private bool _isDashing;
        private float _timeSinceDashStarted = 0f;

        private void Update()
        {
            if (Input.GetKeyDown(dashKey) && !_isDashing)
            {
                if (stats.dashes.value <= 0) return;
                onDash.Invoke();
                _isDashing = true;
                movement.enabled = false;
                _dashDestination = characterTransform.position + movement.direction * dashDistance;
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
                    characterTransform.position = position;
                }
                else
                {
                    // Ensure we reach the exact destination and stop dashing
                    characterTransform.position = _dashDestination;
                    _isDashing = false;
                    movement.enabled = true;
                }
            }
        }

        public void Reset()
        {
            _isDashing = false;
            _timeSinceDashStarted = 0f;
            movement.enabled = true;
        }
    }
}