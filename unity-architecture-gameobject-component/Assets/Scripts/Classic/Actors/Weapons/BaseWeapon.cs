using UnityEngine;

namespace Classic.Actors.Weapons
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public abstract void Attack(MeleeStatsInfo info);
    }
}