using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ProjectileFactory : MonoBehaviour
    {
        [SerializeField]private Projectile projectilePrefab;
        
        public Projectile Create(Vector3 position = new(),Vector3 direction = new(), bool startActive = true)
        {
            var rotation = Quaternion.LookRotation(direction);
            var projectile = Instantiate(projectilePrefab, position, rotation, null);
            projectile.gameObject.SetActive(true);
            return projectile;
        }
    }
}