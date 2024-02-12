using System.Collections.Generic;
using GameObjectComponent.Game;
using GameplayComponents.Combat;
using UnityEngine;

namespace Pools
{
    public class MunitionPool : ScriptableData
    {
        [SerializeField]private MunitionDefinition definition;
        [SerializeField]private GameState gameState;
        private Queue<Munition> _inactivePool = new();
        private List<Munition> _activeProjectiles = new();

        protected override void Init()
        {
            ReturnAllProjectiles();
            _inactivePool.Clear();
            _activeProjectiles.Clear();
        }

        protected override void ResetData()
        {
            ReturnAllProjectiles();
            _inactivePool.Clear();
            _activeProjectiles.Clear();
        }

        private void OnEnable()
        {
            gameState.OnGameLost += ReturnAllProjectiles;
            gameState.OnGameWon += ReturnAllProjectiles;
            gameState.OnGameQuit += ReturnAllProjectiles;
        }
        
        private void OnDisable()
        {
            gameState.OnGameLost -= ReturnAllProjectiles;
            gameState.OnGameWon -= ReturnAllProjectiles;
            gameState.OnGameQuit -= ReturnAllProjectiles;
        }

        public Munition Get(Vector3 position, Vector3 direction, bool startActive = true)
        {
            Munition projectile = null;

            if (_inactivePool.Count == 0)
            {
                projectile = CreateProjectile(position, direction);
            }
            else
            {
                projectile = _inactivePool.Dequeue();
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
        
        public void Return(Munition projectile)
        {
            _activeProjectiles.Remove(projectile);
            projectile.gameObject.SetActive(false);
            _inactivePool.Enqueue(projectile);
        }
        
        private Munition CreateProjectile(Vector3 position, Vector3 direction)
        {
            var projectile = Instantiate(definition.prefab, position, Quaternion.identity, null);
            projectile.transform.forward = direction;
            _activeProjectiles.Add(projectile);
            return projectile;
        }

        private void ReturnAllProjectiles()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.gameObject.SetActive(false);
                _inactivePool.Enqueue(projectile);
            }
        }
    }
}