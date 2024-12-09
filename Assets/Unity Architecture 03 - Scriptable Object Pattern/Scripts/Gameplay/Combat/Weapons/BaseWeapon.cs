using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public abstract class BaseWeapon : ScriptableObject
    {
        public abstract void Attack(WeaponStatsInfo info,CombatTarget target, [DisallowNull]Transform origin);

        public UnityEvent onAttack = new();
    }
}