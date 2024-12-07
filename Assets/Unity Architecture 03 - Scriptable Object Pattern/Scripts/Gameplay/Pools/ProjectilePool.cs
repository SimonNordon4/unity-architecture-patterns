using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField]private ProjectileFactory factory;
        private Queue<Projectile> _inactiveProjectiles = new();
        private List<Projectile> _activeProjectiles = new();

        public void Construct(ProjectilePool newPool)
        {
            _inactiveProjectiles = newPool._inactiveProjectiles;
            _activeProjectiles = newPool._activeProjectiles;
            factory = newPool.factory;
        }

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
            return factory.Create(position, direction);
        }
        
        public void ReturnAllProjectiles()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.gameObject.SetActive(false);
                _inactiveProjectiles.Enqueue(projectile);
            }
        }
    }
}