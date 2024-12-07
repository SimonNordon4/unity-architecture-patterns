using System.Collections;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(KnockBackReceiver))]
    [RequireComponent(typeof(MoveBase))]
    public class KnockBack : MonoBehaviour
    {
        [field:SerializeField]public bool CanBeKnockedBack { get; set; } = true;

        [SerializeField] private float knockBackFactor = 1f;
        private MoveBase _defaultMovement;
        private Movement _movement;
        private KnockBackReceiver _knockBackReceiver;
        private UnityEngine.Transform _transform;

        
        private void Awake()
        {
            _defaultMovement = GetComponent<MoveBase>();
            _movement = GetComponent<Movement>();
            _knockBackReceiver = GetComponent<KnockBackReceiver>();
            _transform = transform;
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
            if(CanBeKnockedBack == false) return;
            if(knockBackFactor <= 0) return;
            // We scale down by 10% for balancing.
            StartCoroutine(KnockBackRoutine(knockBackVector * 0.1f));
        }
        
        private IEnumerator KnockBackRoutine(Vector3 knockBackVector)
        {
            _defaultMovement.enabled = false;
            var knockBackTime = 0.20f * knockBackVector.magnitude;
            var elapsedTime = 0f;

            var originalPosition = _transform.position;
            var targetPosition = originalPosition + knockBackVector * knockBackFactor;

            while (elapsedTime < knockBackTime)
            {
                var normalizedTime = elapsedTime / knockBackTime;
                normalizedTime = 1 - Mathf.Pow(1 - normalizedTime, 3);
        
                var nextPosition = Vector3.Lerp(originalPosition, targetPosition, normalizedTime);
                
                var velocity = (nextPosition - _transform.position) / Time.deltaTime;
                
                // clamp the velocity to a maximum value of 200
                velocity = Vector3.ClampMagnitude(velocity, 200);
                
                _movement.SetVelocity(velocity);
        
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            _defaultMovement.enabled = true;
        }
    }
}