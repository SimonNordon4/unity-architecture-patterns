using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public abstract void Attack(WeaponStatsInfo info, CombatTarget target);

        public UnityEvent onAttack = new();
    }
}