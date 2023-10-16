using System.Collections;
using Classic.Actors;
using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyKnockBack : MonoBehaviour
    {
        [SerializeField] private KnockBackReceiver knockBackReceiver;
        [SerializeField] private EnemyStats stats;
        [SerializeField] private EnemyScope scope;

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
            if(stats.knockBackFactor <= 0) return;
            StartCoroutine(KnockBackRoutine(knockBackVector));
        }
        
        protected virtual IEnumerator KnockBackRoutine(Vector3 knockBackVector)
        {
            float knockBackTime = 0.20f * knockBackVector.magnitude;
            float elapsedTime = 0f;

            Vector3 originalPosition = transform.position;
            Vector3 targetPosition = originalPosition + knockBackVector * stats.knockBackFactor;

            while (elapsedTime < knockBackTime)
            {
                // first we need to normalize the elapsed time.
                var normalizedTime = elapsedTime / knockBackTime;
            
                // Apply cubic easing-out function to the normalized time.
                normalizedTime = 1 - Mathf.Pow(1 - normalizedTime, 3);
            
                // Now we can lerp via the normalized time.
                transform.position = Vector3.Lerp(originalPosition, targetPosition, normalizedTime);
            
                // if the position is over the boundary, clamp it back to the boundary
                var pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, -scope.level.bounds.x, scope.level.bounds.x);
                pos.z = Mathf.Clamp(pos.z, -scope.level.bounds.y, scope.level.bounds.y);
                transform.position = pos;
            
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPosition;
        }
    }
}