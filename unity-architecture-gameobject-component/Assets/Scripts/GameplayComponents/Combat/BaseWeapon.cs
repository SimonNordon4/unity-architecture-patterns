using GameplayComponents.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Combat
{
    public abstract class BaseWeapon : MonoBehaviour
    {

        public virtual void Attack(WeaponStatsInfo info, CombatTarget target = null)
        {
            
        }
        public UnityEvent onAttack = new();
    }
}