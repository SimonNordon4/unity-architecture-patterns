using GameObjectComponent.Game;
using GameplayComponents.Life;
using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class Projectile : GameplayComponent
    {
        private GameState _state;
        private ProjectilePool _pool;
        
        private LayerMask _targetLayer;
        private float _speed;
        private int _damage;
        private float _knockBackForce;
        private float _pierceValue;

        [SerializeField] private float lifeTime;

        private Transform _projectileTransform;
        private float _timeAlive = 0f;

        private void Start()
        {
            _projectileTransform = transform;
        }

        public void SetPool(ProjectilePool pool)
        {
            _pool = pool;
        }

        public void Set(LayerMask targetLayer = default, float speed = 10f, int damage = 1,
            float knockBackForce = 1f, int pierce = 0)
        {
            _targetLayer = targetLayer;
            _speed = speed;
            _damage = damage;
            _knockBackForce = knockBackForce;
            _pierceValue = pierce;
        }

        private void Update()
        {
            if (_timeAlive < lifeTime)
            {
                _timeAlive += Time.deltaTime;
                _projectileTransform.position += _projectileTransform.forward * (_speed * Time.deltaTime);
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
                _pierceValue--;
                if (_pierceValue <= 0)
                {
                    EndProjectile();
                }
            }

            if (other.TryGetComponent(out KnockBackReceiver knockBackReceiver))
            {
                knockBackReceiver.ApplyKnockBack(_projectileTransform.forward * _knockBackForce);
            }
        }

        private void EndProjectile()
        {
            // TODO: Play death animation, then return to pool
            
            if (_pool != null)
            {
                _pool.Return(this);
                return;
            }
            Destroy(gameObject);
        }
    }
}