using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 3f;
        private float _timeAlive = 0f;
        private Transform _projectileTransform;

        private ProjectilePool _pool;
        
        private LayerMask _targetLayer;
        private float _speed;
        private int _damage;
        private float _knockBackForce;
        private float _pierceValue;
        private bool _isCrit;
        
        public void Construct(ProjectilePool pool)
        {
            _pool = pool;
        }

        private void OnEnable()
        {
            _timeAlive = 0f;
        }

        private void Start()
        {
            _projectileTransform = transform;
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

        public void Set(LayerMask targetLayer = default, float speed = 10f, int damage = 1,
            float knockBackForce = 1f, int pierce = 0, bool isCrit = false)
        {
            _targetLayer = targetLayer;
            _speed = speed;
            _damage = damage;
            _knockBackForce = knockBackForce;
            _pierceValue = pierce;
            _isCrit = isCrit;
        }

        public void Set(WeaponStatsInfo info, LayerMask targetLayer = default, float speed = 10f)
        {
            _targetLayer = targetLayer;
            _speed = speed;
            _damage = info.Damage;
            _knockBackForce = info.KnockBack;
            _pierceValue = info.Pierce;
            _isCrit = info.IsCrit;
        }

        private void OnTriggerEnter(Collider other)
        {
            // check if other is in target layer
            if (_targetLayer != (_targetLayer | (1 << other.gameObject.layer))) return;
            

            if (other.TryGetComponent(out KnockBackReceiver knockBackReceiver))
            {
                knockBackReceiver.ApplyKnockBack(_projectileTransform.forward * _knockBackForce);
            }

            // check if other has a damage receiver
            if (other.TryGetComponent(out DamageReceiver damageReceiver))
            {
                damageReceiver.TakeDamage(_isCrit ? _damage * 2 : _damage, _isCrit);
                if (_pierceValue <= 0)
                {
                    EndProjectile();
                }
                
                _pierceValue--;
            }

        }
        
        public virtual void EndProjectile()
        {
            _pool.Return(this);
        }
    }
}