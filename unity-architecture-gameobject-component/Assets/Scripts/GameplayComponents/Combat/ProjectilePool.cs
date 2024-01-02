using System.Collections.Generic;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class ProjectilePool : GameplayComponent
    {
        [SerializeField] private ProjectileDefinition projectileDefinition;
        [SerializeField] private int poolSize = 10;

        private readonly Queue<Projectile> _inactiveProjectiles = new();
        private List<Projectile> _activeProjectiles = new();

        private void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var newProjectile = CreateNewProjectile();
                newProjectile.gameObject.SetActive(false);
                _inactiveProjectiles.Enqueue(newProjectile);
            }
        }

        public Projectile Get(Vector3 spawnPoint, Vector3 direction)
        {
            var projectile = _inactiveProjectiles.Count <= 0 ? CreateNewProjectile() : _inactiveProjectiles.Dequeue();
            projectile.transform.position = spawnPoint;
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.gameObject.SetActive(true);
            _activeProjectiles.Add(projectile);
            return projectile;
        }

        private Projectile CreateNewProjectile()
        {
            var newProjectile = Instantiate(projectileDefinition.prefab, null);
            newProjectile.SetPool(this);
            
            return newProjectile;
        }
        
        public void Return(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            _activeProjectiles.Remove(projectile);
            _inactiveProjectiles.Enqueue(projectile);
        }

        private void ReturnAllProjectiles()
        {
            foreach(var projectile in _activeProjectiles)
                Return(projectile);
        }

        private void DeletePool()
        {
            ReturnAllProjectiles();
            while (_inactiveProjectiles.Count > 0)
            {
                var projectile = _inactiveProjectiles.Dequeue();
                Destroy(projectile.gameObject);
            }
        }

        public override void OnGameEnd()
        {
            DeletePool();
        }
    }
}

