using UnityEngine;


namespace UnityArchitecture.ScriptableObjectPattern
{
    public class PistolWeapon : BaseWeapon
    {
        [SerializeField]private ProjectilePool projectilePool;
        [SerializeField]private UnityEngine.Transform projectileSpawnPoint;
        [SerializeField]private float projectileSpeed = 10f;
                
        public override void Attack(WeaponStatsInfo info, CombatTarget target, Transform origin)
        {
            var projectile = projectilePool.Get(projectileSpawnPoint.position, target.TargetDirection);
            projectile.Set(target.targetLayer, projectileSpeed, info.Damage, info.KnockBack, info.Pierce);
            onAttack.Invoke();
        }
    }
}