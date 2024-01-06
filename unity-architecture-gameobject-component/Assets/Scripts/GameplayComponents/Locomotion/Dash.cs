using System.Collections;
using GameObjectComponent.Game;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Locomotion
{
    public class Dash : GameplayComponent
    {
        [SerializeField] private float dashDistance = 5f;
        [SerializeField] private float dashTime = 0.2f;
        [SerializeField] private Stats stats;
        [SerializeField] private Level level;
        [SerializeField] private DamageHandler damageHandler;
        private bool hasDamageHandler = false;
        
        private bool _isDashing;
        private Stat _dashes;
        
        public UnityEvent onDashStart = new();
        public UnityEvent onDashEnd = new();

        private void Start()
        {
            _dashes = stats.GetStat(StatType.Dashes);
            
            hasDamageHandler = damageHandler != null;
        }

        public void DashForward()
        {
            if(_isDashing || _dashes.value <= 0) return;
            onDashStart.Invoke();
            StartCoroutine(DashForwardCoroutine());
        }

        private IEnumerator DashForwardCoroutine()
        {
            _isDashing = true;
            _dashes.value--;
            
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
                transform.position = new Vector3(Mathf.Clamp(desiredPos.x, -level.bounds.x, level.bounds.x),
                    transform.position.y,
                    Mathf.Clamp(desiredPos.z, -level.bounds.y, level.bounds.y));
                
                yield return new WaitForEndOfFrame();
            }
            
            onDashEnd.Invoke();
            _isDashing = false;
        }
        
        public override void OnGameEnd()
        {
            StopAllCoroutines();
            _isDashing = false;
            
        }
    }
}