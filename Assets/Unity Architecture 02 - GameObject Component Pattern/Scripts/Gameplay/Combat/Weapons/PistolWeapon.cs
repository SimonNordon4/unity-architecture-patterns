﻿using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class PistolWeapon : BaseWeapon
    {
        [SerializeField]private ProjectilePool projectilePool;
        [SerializeField]private Transform projectileSpawnPoint;
        [SerializeField]private float projectileSpeed = 10f;
                
        public override void Attack(WeaponStatsInfo info, CombatTarget target)
        {
            var projectile = projectilePool.Get(projectileSpawnPoint.position, target.TargetDirection);
            projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            onAttack.Invoke();
        }
    }
}