using GameObjectComponent.Game;
using GameplayComponents.Life;
using GameplayComponents.Locomotion;
using Pools;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class Projectile : Munition
    {
        private Transform _projectileTransform;
        private float _timeAlive = 0f;

        private void Start()
        {
            _projectileTransform = transform;
        }

        // We need to set the time alive back to zero when it's reset, otherwise it instantly dies.
        private void OnEnable()
        {
            _timeAlive = 0f;
        }
        
        private void Update()
        {
            if (_timeAlive < lifeTime)
            {
                _timeAlive += GameTime.deltaTime;
                _projectileTransform.position += _projectileTransform.forward * (_speed * GameTime.deltaTime);
            }
            else
            {
               EndProjectile();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // check if other is in target layer
            if (_targetLayer != (_targetLayer | (1 << other.gameObject.layer))) return;
            
            // check if other has a damage receiver
            if (other.TryGetComponent(out DamageReceiver damageReceiver))
            {
                damageReceiver.TakeDamage(_damage);
                if (_pierceValue <= 0)
                {
                    EndProjectile();
                }
                
                _pierceValue--;
            }

            if (other.TryGetComponent(out KnockBackReceiver knockBackReceiver))
            {
                knockBackReceiver.ApplyKnockBack(_projectileTransform.forward * _knockBackForce);
            }
        }

        public override void EndProjectile()
        {
            _timeAlive = 0f;
            if (_pool != null)
            {
                _pool.Return(this, definition);
                return;
            }
            Destroy(gameObject);
        }
    }
}