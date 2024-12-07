using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ProjectilePool : ScriptableObject
    {
        [SerializeField]private GameObject projectilePrefab;
        private readonly Queue<GameObject> _inactiveProjectiles = new();
        private readonly List<GameObject> _activeProjectiles = new();

        private void Awake()
        {
            if (!projectilePrefab.TryGetComponent<Projectile>(out var projectile))
            {
                Debug.LogError("Projectile Prefab doesn't contain a Projectile component.", this);
            }
        }

        public Projectile Get(Vector3 position, Vector3 direction, bool startActive = true)
        {
            GameObject projectile = null;

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
            
            projectile.gameObject.SetActive(startActive);
            _activeProjectiles.Add(projectile);
            return projectile.GetComponent<Projectile>();
        }
        
        public void Return(GameObject projectile)
        {
            _activeProjectiles.Remove(projectile);
            projectile.gameObject.SetActive(false);
            _inactiveProjectiles.Enqueue(projectile);
        }
        
        private GameObject CreateProjectile(Vector3 position, Vector3 direction)
        {
            return Instantiate(projectilePrefab, position, Quaternion.LookRotation(direction));
        }
    }
}