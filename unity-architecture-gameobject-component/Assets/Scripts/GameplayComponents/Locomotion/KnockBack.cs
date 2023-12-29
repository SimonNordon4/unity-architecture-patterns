using System.Collections;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(KnockBackReceiver))]
    public class KnockBack : MonoBehaviour
    {
        [SerializeField] private float knockBackFactor = 1f;
        [SerializeField] private GameplayComponent defaultMovement;
        private Movement _movement;
        private KnockBackReceiver _knockBackReceiver;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _knockBackReceiver = GetComponent<KnockBackReceiver>();
            _movement = GetComponent<Movement>();
        }
        
        private void OnEnable()
        {
            _knockBackReceiver.OnKnockBack += OnKnockBack;
        }
        
        private void OnDisable()
        {
            _knockBackReceiver.OnKnockBack -= OnKnockBack;
        }

        private void OnKnockBack(Vector3 knockBackVector)
        {
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
                
                _movement.SetVelocity(velocity);
        
                elapsedTime += GameTime.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            defaultMovement.enabled = true;
        }
    }
}