using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayComponents;
using GameplayComponents.Combat;
using UnityEngine;

namespace Pools
{
    public class MunitionPool : GameplayComponent
    {
        [SerializeField]private MunitionFactory factory;
        
        
        private Dictionary<MunitionDefinition, Queue<Munition>> _inactivePools = new();
        private List<Munition> _activeProjectiles = new();

        public void Construct(MunitionPool parentPool)
        {
            factory = parentPool.factory;
            _inactivePools = parentPool._inactivePools;
            _activeProjectiles = parentPool._activeProjectiles;
        }

        public Munition Get([DisallowNull]MunitionDefinition definition, Vector3 position, Vector3 direction, bool startActive = true)
        {
            if (!_inactivePools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<Munition>();
                _inactivePools.Add(definition, queue);
            }
            
            Munition projectile = null;

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
            projectile.gameObject.SetActive(startActive);
            projectile.enabled = startActive;

            _activeProjectiles.Add(projectile);
            return projectile;
        }
        
        public void Return(Munition projectile, MunitionDefinition definition)
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
        
        private Munition CreateProjectile(MunitionDefinition definition, Vector3 position, Vector3 direction)
        {
            // Check if there exists a pool
            if (!_inactivePools.TryGetValue(definition, out var queue))
            {
                queue = new Queue<Munition>();
                _inactivePools.Add(definition, queue);
            }
            
            return factory.Create(definition, position, direction);
        }
        
        public void ReturnAllProjectiles()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.gameObject.SetActive(false);
                _inactivePools[projectile.definition].Enqueue(projectile);
            }
        }

        public override void OnGameEnd()
        {
            ReturnAllProjectiles();
        }
    }
}