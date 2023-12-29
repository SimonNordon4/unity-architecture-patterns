using System.Collections;
using Classic.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Character
{
    public class CharacterSwordAttack : MonoBehaviour
    {
        public UnityEvent onSwing = new();
        
        [SerializeField] private CharacterTarget target;
        [SerializeField] private Transform swordPivot;
        [SerializeField] private Transform transformToFollow;
        [SerializeField] private Stats stats;
        
        
        private bool _isSwingingLeftToRight = true;
        private float _timeSinceLastAttack = 0f;
        private bool _isAttacking = false;

        private void Start()
        {
            swordPivot.gameObject.SetActive(false);
            swordPivot.transform.parent = null;
        }

        private void Update()
        {
            swordPivot.transform.position = transformToFollow.position;
            _timeSinceLastAttack += Time.deltaTime;
            if (_timeSinceLastAttack < stats.attackSpeed.value) return;
            if (_isAttacking) return;
            if (target.closestTransform is null) return;
            if (target.distance > stats.meleeRange.value) return;
            
            StartCoroutine(SwordAttack());
            onSwing.Invoke();
            _timeSinceLastAttack = 0f;
        }

        private IEnumerator SwordAttack()
        {
            _isAttacking = true;
            var swordArc = 45f;
            // Enable the sword gameobject.
            swordPivot.gameObject.SetActive(true);
            swordPivot.localScale = new Vector3(1f, 1f, stats.meleeRange.value);

            // Base rotation values.
            var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
            var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);

            swordPivot.forward = target.targetDirection;

            // Determine the start and end rotation based on the current swing direction.
            Quaternion startRotation, endRotation;
            if (_isSwingingLeftToRight)
            {
                startRotation = Quaternion.LookRotation(target.targetDirection) * leftRotation;
                endRotation = Quaternion.LookRotation(target.targetDirection) * rightRotation;
            }
            else
            {
                startRotation = Quaternion.LookRotation(target.targetDirection) * rightRotation;
                endRotation = Quaternion.LookRotation(target.targetDirection) * leftRotation;
            }

            var total180Arcs = Mathf.FloorToInt(swordArc / 180f);
            var swingTime = stats.meleeRange.value * 0.2f;
            
            Debug.Log($"Swing time: {swingTime}");

            if (total180Arcs > 0)
            {
                var lastStart = startRotation;
                var directionSign = _isSwingingLeftToRight ? 1 : -1;
                var lastEnd = startRotation * Quaternion.Euler(0, 179.9f * directionSign, 0);

                for (var i = 0; i < total180Arcs; i++)
                {
                    var t = 0.0f;
                    var swing = true;
                    while (swing)
                    {
                        t += Time.deltaTime;
                        swordPivot.rotation = Quaternion.Lerp(lastStart, lastEnd, t / swingTime);
                        yield return new WaitForEndOfFrame();
                        if (!(t >= swingTime)) continue;
                        lastStart = swordPivot.rotation;
                        lastEnd = lastStart * Quaternion.Euler(0, 179.9f * directionSign, 0);
                        swing = false;
                    }
                }
            }
            else
            {
                // Lerp the sword rotation from start to end over 0.5 seconds.
                var t = 0.0f;

                while (t < swingTime)
                {
                    t += GameTime.deltaTime;
                    swordPivot.rotation = Quaternion.Lerp(startRotation, endRotation, t / swingTime);
                    yield return new WaitForEndOfFrame();
                }
            }

            // Toggle the swing direction for the next attack.
            _isSwingingLeftToRight = !_isSwingingLeftToRight;

            _isAttacking = false;

            // Disable the sword gameobject.
            swordPivot.gameObject.SetActive(false);
        }

        public void Reset()
        {
            swordPivot.forward = transformToFollow.forward;
            _timeSinceLastAttack = 0f;
            _isAttacking = false;
            swordPivot.gameObject.SetActive(false);
        }
    }
}