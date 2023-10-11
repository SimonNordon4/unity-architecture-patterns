using System.Collections;
using Classic.Game;
using Classic.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Character
{
    public class CharacterDash : MonoBehaviour, IResettable
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Stats stats;
        [SerializeField] private KeyCode dashKey = KeyCode.Space;
        [SerializeField] private Level level;

        private Vector3 _dashDestination;
        [SerializeField] private float dashDistance = 1f;
        [SerializeField] private float dashTime = 0.5f;
        
        public UnityEvent onDash = new();
        private bool _isDashing;

        private void Update()
        {
            if (Input.GetKeyDown(dashKey) && !_isDashing)
            {
                if (stats.dashes.value <= 0) return;
                onDash.Invoke();
                StartCoroutine(Dash());
            }

        }

        private IEnumerator Dash()
        {
            _isDashing = true;
            stats.dashes.value--;
            
            var elapsedTime = 0f;
            var startPosition = transform.position;
            var dashDestination = transform.forward * dashDistance + transform.position;
            
            while (elapsedTime < dashTime)
            {
                elapsedTime += Time.deltaTime;

                var normalizedTime = elapsedTime / dashTime;
                var inverseQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
                
                var desiredPos = Vector3.Lerp(startPosition, dashDestination, inverseQuadraticTime);
                
                // clamp the position to the level bounds
                characterTransform.position = new Vector3(Mathf.Clamp(desiredPos.x, -level.bounds.x, level.bounds.x),
                    characterTransform.position.y,
                    Mathf.Clamp(desiredPos.z, -level.bounds.y, level.bounds.y));
                
                if (gameState.currentState != GameStateEnum.Active)
                {
                    _isDashing = false;
                    yield break;
                }
                
                yield return new WaitForEndOfFrame();
            }
            
            _isDashing = false;

        }

        public void Reset()
        {
            _isDashing = false;
        }
    }
}