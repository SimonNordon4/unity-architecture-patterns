using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ProjectilePool : ScriptableObject
    {
        [SerializeField]private Projectile projectilePrefab;
        private readonly Queue<Projectile> _inactiveProjectiles = new();
        private readonly List<Projectile> _activeProjectiles = new();

        public Projectile Get(Vector3 position, Vector3 direction, bool startActive = true)
        {
            Projectile projectile = null;

            if (_inactiveProjectiles.Count == 0)
            {
                projectile = CreateProjectile(position, direction);
            }
            else
            {
                projectile = _inactiveProjectiles.Dequeue();
                var projectileTransform = projectile.transform;
                projectileTransform.position = position;
                projectileTransform.forward = direction;
                projectile.gameObject.SetActive(startActive);
            }
            
            projectile.Construct(this);
            projectile.gameObject.SetActive(startActive);
            projectile.enabled = startActive;

            _activeProjectiles.Add(projectile);
            return projectile;
        }
        
        public void Return(Projectile projectile)
        {
            _activeProjectiles.Remove(projectile);
            projectile.gameObject.SetActive(false);
            _inactiveProjectiles.Enqueue(projectile);
        }
        
        private Projectile CreateProjectile(Vector3 position, Vector3 direction)
        {
            return Instantiate(projectilePrefab, position, Quaternion.LookRotation(direction));
        }
    }
}