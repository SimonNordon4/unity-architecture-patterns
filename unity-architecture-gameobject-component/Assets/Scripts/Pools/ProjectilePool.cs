using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayComponents;
using GameplayComponents.Combat;
using UnityEngine;

namespace Pools
{
    public class ProjectilePool : GameplayComponent
    {
        [SerializeField]private ProjectileFactory factory;
        
        
        private Dictionary<ProjectileDefinition, Queue<Projectile>> _inactivePools = new();
        private List<Projectile> _activeProjectiles = new();

        public void Construct(ProjectilePool parentPool)
        {
            factory = parentPool.factory;
            _inactivePools = parentPool._inactivePools;
            _activeProjectiles = parentPool._activeProjectiles;
        }

        public Projectile Get([DisallowNull]ProjectileDefinition definition, Vector3 position, Vector3 direction, bool startActive = true)
        {
            if (!_inactivePools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<Projectile>();
                _inactivePools.Add(definition, queue);
            }
            
            Projectile projectile = null;

            if (queue.Count == 0)
            {
                projectile = CreateProjectile(definition, position, direction);
            }
            else
            {
                projectile = queue.Dequeue();
                var projectileTransform = projectile.transform;
                projectileTransform.position = position;
                projectileTransform.forward = direction;
                projectile.gameObject.SetActive(startActive);
            }
            
            projectile.Construct(this);

            _activeProjectiles.Add(projectile);
            return projectile;
        }
        
        public void Return(Projectile projectile, ProjectileDefinition definition)
        {
            // check if definition is null
            if (definition == null)
            {
                Debug.LogError("Projectile definition is null");
                return;
            }
            _activeProjectiles.Remove(projectile);
            projectile.gameObject.SetActive(false);
            _inactivePools[definition].Enqueue(projectile);
        }
        
        private Projectile CreateProjectile(ProjectileDefinition definition, Vector3 position, Vector3 direction)
        {
            // Check if there exists a pool
            if (!_inactivePools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<Projectile>();
                _inactivePools.Add(definition, queue);
            }
            
            return factory.Create(definition, position, direction);
        }
        
        public void ReturnAllProjectiles()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.gameObject.SetActive(false);
                _inactivePools[projectile.projectileDefinition].Enqueue(projectile);
            }
        }

        public override void OnGameEnd()
        {
            ReturnAllProjectiles();
        }
    }
}