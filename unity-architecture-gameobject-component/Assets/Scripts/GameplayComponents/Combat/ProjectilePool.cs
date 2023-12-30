using System.Collections.Generic;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class ProjectilePool : GameplayComponent
    {
        [SerializeField] private ProjectileDefinition projectileDefinition;
        [SerializeField] private int poolSize = 10;

        private Queue<Projectile> _projectilePool = new();

        private void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var newProjectile = CreateNewProjectile();
                newProjectile.gameObject.SetActive(false);
                _projectilePool.Enqueue(newProjectile);
            }
        }

        public Projectile Spawn(Vector3 spawnPoint, Vector3 direction)
        {
            var projectile = _projectilePool.Count <= 0 ? CreateNewProjectile() : _projectilePool.Dequeue();
            projectile.transform.position = spawnPoint;
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.gameObject.SetActive(true);
            return projectile;
        }

        private Projectile CreateNewProjectile()
        {
            var newProjectile = Instantiate(projectileDefinition.prefab, null);
            newProjectile.SetPool(this);
            return newProjectile;
        }
        
        public void Despawn(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            _projectilePool.Enqueue(projectile);
        }
    }
}

