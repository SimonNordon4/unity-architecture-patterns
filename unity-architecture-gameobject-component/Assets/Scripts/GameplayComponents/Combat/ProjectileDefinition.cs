using UnityEngine;

namespace GameplayComponents.Combat
{
    [CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Classic/Projectile Definition")]
    public class ProjectileDefinition : ScriptableObject
    {
        public Projectile prefab;
    }
}