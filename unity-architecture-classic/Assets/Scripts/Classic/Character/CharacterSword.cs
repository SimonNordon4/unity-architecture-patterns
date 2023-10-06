// using System.Collections;
// using Classic.Actor;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace Classic.Character
// {
//     public class CharacterSwordAttack : MonoBehaviour
//     {
//         public UnityEvent onSwing = new();
//         
//         [SerializeField] private CharacterTarget target;
//         [SerializeField] private Transform swordPivot;
//         
//         private bool _isSwingingLeftToRight = true;
//         private float _timeSinceLastAttack = 0f;
//         private bool _isAttacking = false;
//
//         private void Start()
//         {
//             swordPivot.gameObject.SetActive(false);
//             swordPivot.transform.parent = null;
//         }
//
//         private void Update()
//         {
//             swordPivot.transform.position = transform.position;
//             _timeSinceLastAttack += Time.deltaTime;
//             if (_timeSinceLastAttack < GameManager.instance.swordAttackSpeed.value) return;
//             if (_isAttacking) return;
//             if (target.closestTransform is null) return;
//             if (target.distance > GameManager.instance.swordRange.value) return;
//             
//             StartCoroutine(SwordAttack());
//             onSwing.Invoke();
//             _timeSinceLastAttack = 0f;
//         }
//
//         private IEnumerator SwordAttack()
//         {
//             _isAttacking = true;
//             var swordArc = GameManager.instance.swordArc.value;
//             // Enable the sword gameobject.
//             swordPivot.gameObject.SetActive(true);
//             swordPivot.localScale = new Vector3(1f, 1f, GameManager.instance.swordRange.value);
//
//             // Base rotation values.
//             var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
//             var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);
//
//             // The start rotation needs to be directed to the closest target.
//             var directionToTarget = Vector3
//                 .ProjectOnPlane(target.closestTransform.transform.position - transform.position, Vector3.up).normalized;
//             swordPivot.forward = directionToTarget;
//
//             // Determine the start and end rotation based on the current swing direction.
//             Quaternion startRotation, endRotation;
//             if (_isSwingingLeftToRight)
//             {
//                 startRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
//                 endRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
//             }
//             else
//             {
//                 startRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
//                 endRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
//             }
//
//             var total180Arcs = Mathf.FloorToInt(swordArc / 180f);
//             var swingTime = GameManager.instance.swordRange.value * 0.08f;
//
//             if (total180Arcs > 0)
//             {
//                 var lastStart = startRotation;
//                 var directionSign = _isSwingingLeftToRight ? 1 : -1;
//                 var lastEnd = startRotation * Quaternion.Euler(0, 179.9f * directionSign, 0);
//
//                 for (var i = 0; i < total180Arcs; i++)
//                 {
//                     var t = 0.0f;
//                     var swing = true;
//                     while (swing)
//                     {
//                         t += Time.deltaTime;
//                         swordPivot.rotation = Quaternion.Lerp(lastStart, lastEnd, t / swingTime);
//                         yield return null;
//                         if (!(t >= swingTime)) continue;
//                         lastStart = swordPivot.rotation;
//                         lastEnd = lastStart * Quaternion.Euler(0, 179.9f * directionSign, 0);
//                         swing = false;
//                     }
//                 }
//             }
//             else
//             {
//                 // Lerp the sword rotation from start to end over 0.5 seconds.
//                 var t = 0.0f;
//
//                 while (t < swingTime)
//                 {
//                     t += Time.deltaTime;
//                     swordPivot.rotation = Quaternion.Lerp(startRotation, endRotation, t / swingTime);
//                     yield return null;
//                 }
//             }
//
//             // Toggle the swing direction for the next attack.
//             _isSwingingLeftToRight = !_isSwingingLeftToRight;
//
//             _isAttacking = false;
//
//             // Disable the sword gameobject.
//             swordPivot.gameObject.SetActive(false);
//         }
//     }
// }