using System.Collections;
using GameObjectComponent.Game;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents.Combat.Weapon
{
    public class SwordWeapon : BaseWeapon
    {
        private bool _isSwordAttacking = false;
        private bool _isSwingingLeftToRight = true;
        [SerializeField] private Transform swordPivot;
        [SerializeField] private Stats stats;
        private Stat meleeRange => stats.GetStat(StatType.MeleeRange);
        
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            StartCoroutine(SwordSwing(target));
        }
        
        private IEnumerator SwordSwing(CombatTarget target)
        {
            _isSwordAttacking = true;
            var swordArc = 60f;
            // Enable the sword gameobject.
            swordPivot.gameObject.SetActive(true);
            swordPivot.localScale = new Vector3(1f, 1f, meleeRange.value);
        
            // Base rotation values.
            var leftRotation = Quaternion.Euler(0, swordArc * -0.5f, 0);
            var rightRotation = Quaternion.Euler(0, swordArc * 0.5f, 0);
        
            // The start rotation needs to be directed to the closest target.
            var directionToTarget = target.targetDirection;
            swordPivot.forward = directionToTarget;
        
            // Determine the start and end rotation based on the current swing direction.
            Quaternion startRotation, endRotation;
            if (_isSwingingLeftToRight)
            {
                startRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
                endRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
            }
            else
            {
                startRotation = Quaternion.LookRotation(directionToTarget) * rightRotation;
                endRotation = Quaternion.LookRotation(directionToTarget) * leftRotation;
            }
            
            var total180Arcs = Mathf.FloorToInt(swordArc / 180f);
            var swingTime = meleeRange.value * 0.12f;

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
                        t += GameTime.deltaTime;
                        swordPivot.rotation = Quaternion.Lerp(lastStart, lastEnd, t / swingTime);
                        yield return null;
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
                    yield return null;
                }
            }

            _isSwordAttacking = false;
        
            // Toggle the swing direction for the next attack.
            _isSwingingLeftToRight = !_isSwingingLeftToRight;
        
            // Disable the sword gameobject.
            swordPivot.gameObject.SetActive(false);
        }
    }
}