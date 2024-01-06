using GameplayComponents.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayComponents.Combat.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour
    {

        public abstract void Attack(WeaponStatsInfo info, CombatTarget target = null);

        public UnityEvent onAttack = new();
    }
}