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
        [field:SerializeField] public float dashDistance = 5f;
        [field:SerializeField] public float dashTime = 0.2f;
        [SerializeField] private Stats stats;
        [SerializeField] private Movement movement;
        [SerializeField] private GameplayComponent defaultMovement;
        
        private bool _isDashing;
        private Stat _dashes;
        
        public UnityEvent onDashStart = new();
        public UnityEvent onDashEnd = new();

        private void Start()
        {
            _dashes = stats.GetStat(StatType.Dashes);
        }

        public void DashForward()
        {
            if(_isDashing || _dashes.value <= 0)
            {
                return;
            }

            
            defaultMovement.enabled = false;
            onDashStart.Invoke();
       
            StartCoroutine(DashForwardCoroutine());
        }

        private IEnumerator DashForwardCoroutine()
        {
            _isDashing = true;
            _dashes.value--;
            
            var elapsedTime = 0f;
            var trans = transform;
            var startPosition = trans.position;
            var dashDestination = trans.forward * dashDistance + startPosition;
            
           
            var desiredVelocity = (dashDestination - startPosition) / dashTime;

            
            while (elapsedTime < dashTime)
            {
                elapsedTime += GameTime.deltaTime;
                movement.SetVelocity(desiredVelocity);
                yield return new WaitForEndOfFrame();
            }
            
            onDashEnd.Invoke();
            _isDashing = false;
            defaultMovement.enabled = true;
        }
        
        public override void OnGameEnd()
        {
            StopAllCoroutines();
            _isDashing = false;
        }
    }
}