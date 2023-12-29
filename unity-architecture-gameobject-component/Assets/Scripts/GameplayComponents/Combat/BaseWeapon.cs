using UnityEngine;

namespace GameObjectComponent.GameplayComponents.Combat
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public abstract void Attack(MeleeStatsInfo info);
    }
}