using System.Collections;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class KnockBack : GameplayComponent
    {
        [SerializeField] private float knockBackFactor = 1f;
        [SerializeField] private GameplayComponent defaultMovement;
        [SerializeField] private Movement movement;
        [SerializeField] private KnockBackReceiver knockBackReceiver;
        [SerializeField] private Transform _transform;

        [field:SerializeField]public bool canBeKnockedBack { get; set; } = true;
        private bool _initialCanKnockBack;

        private void Start()
        {
            _initialCanKnockBack = canBeKnockedBack;
        }

        private void OnEnable()
        {
            knockBackReceiver.OnKnockBack += OnKnockBack;
        }
        
        private void OnDisable()
        {
            knockBackReceiver.OnKnockBack -= OnKnockBack;
        }

        private void OnKnockBack(Vector3 knockBackVector)
        {
            if(canBeKnockedBack == false) return;
            if(knockBackFactor <= 0) return;
            StartCoroutine(KnockBackRoutine(knockBackVector));
        }
        
        private IEnumerator KnockBackRoutine(Vector3 knockBackVector)
        {
            defaultMovement.enabled = false;
            var knockBackTime = 0.20f * knockBackVector.magnitude;
            var elapsedTime = 0f;

            var originalPosition = _transform.position;
            var targetPosition = originalPosition + knockBackVector * knockBackFactor;

            while (elapsedTime < knockBackTime)
            {
                var normalizedTime = elapsedTime / knockBackTime;
                normalizedTime = 1 - Mathf.Pow(1 - normalizedTime, 3);
        
                var nextPosition = Vector3.Lerp(originalPosition, targetPosition, normalizedTime);
                
                var velocity = (nextPosition - _transform.position) / GameTime.deltaTime;
                
                // clamp the velocity to a maximum value of 200
                velocity = Vector3.ClampMagnitude(velocity, 200);
                
                movement.SetVelocity(velocity);
        
                elapsedTime += GameTime.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            defaultMovement.enabled = true;
        }

        public override void OnGameStart()
        {
            canBeKnockedBack = _initialCanKnockBack;
        }
    }
}