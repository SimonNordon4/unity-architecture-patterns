using Pools;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public abstract class Munition : GameplayComponent
    {
        [SerializeField] protected float lifeTime = 3f;
        [field:SerializeField] public ProjectileDefinition definition { get; private set; }
        protected MunitionPool _pool;
        
        protected LayerMask _targetLayer;
        protected float _speed;
        protected int _damage;
        protected float _knockBackForce;
        protected float _pierceValue;
        
        public virtual void Construct(MunitionPool pool)
        {
            _pool = pool;
        }

        public virtual void Set(LayerMask targetLayer = default, float speed = 10f, int damage = 1,
            float knockBackForce = 1f, int pierce = 0)
        {
            _targetLayer = targetLayer;
            _speed = speed;
            _damage = damage;
            _knockBackForce = knockBackForce;
            _pierceValue = pierce;
        }
        
        public virtual void EndProjectile()
        {
            _pool.Return(this, definition);
        }
    }
}