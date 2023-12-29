using GameplayComponents.Combat;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public abstract void Attack(MeleeStatsInfo info);
    }
}