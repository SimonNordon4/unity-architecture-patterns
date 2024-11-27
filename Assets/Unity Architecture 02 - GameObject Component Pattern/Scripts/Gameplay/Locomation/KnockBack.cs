using System.Collections;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(KnockBackReceiver))]
    public class KnockBack : MonoBehaviour
    {
        [field:SerializeField]public bool CanBeKnockedBack { get; set; } = true;

        [SerializeField] private float knockBackFactor = 1f;
        [SerializeField] private MonoBehaviour defaultMovement;

        private Movement _movement;
        private KnockBackReceiver _knockBackReceiver;
        private Transform _transform;

        

        private void Awake()
        {
            if(defaultMovement == null)
            {
                Debug.LogError("Default Movement can not be none", this);
            }

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
                
                var velocity = (nextPosition - _transform.position) / Time.deltaTime;
                
                // clamp the velocity to a maximum value of 200
                velocity = Vector3.ClampMagnitude(velocity, 200);
                
                _movement.SetVelocity(velocity);
        
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            defaultMovement.enabled = true;
        }
    }
}