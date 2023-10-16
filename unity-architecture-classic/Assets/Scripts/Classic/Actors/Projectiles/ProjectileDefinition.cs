using UnityEngine;

namespace Classic.Actors
{
    [CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Classic/Projectile Definition")]
    public class ProjectileDefinition : ScriptableObject
    {
        public Projectile prefab;
    }
}