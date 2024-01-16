using System.Collections;
using GameObjectComponent.Game;
using GameplayComponents.Life;
using UnityEngine;

namespace GameplayComponents.Combat
{
    public class Bomb : Munition
    {
        [SerializeField]private ParticleSystem explosionEffect;
        [SerializeField]private float explosionRadius = 3f;
        private bool _exploded = false;
        
        private readonly Collider[] _results = new Collider[10];
        
        private float _timer;

        private void Update()
        {
            _timer += GameTime.deltaTime;
            if (_timer > lifeTime && !_exploded)
            {
                _exploded = true;
                Explode();
                _timer = 0;
            }
        }

        private void Explode()
        {
            StartCoroutine(PlayEffect());
            var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, _results, _targetLayer);
            if(size == 0) return;
            
            foreach (var col in _results)
            {
                Debug.Log("Found collider " + col.name);
                if (col.TryGetComponent<DamageReceiver>(out var damageReceiver))
                {
                    damageReceiver.TakeDamage(_damage);
                }
            }
        }

        private IEnumerator PlayEffect()
        {
            Debug.Log("Playing effect");
            explosionEffect.Play();
            yield return new WaitForSeconds(explosionEffect.main.duration);
            EndProjectile();
        }
        
        public override void EndProjectile()
        {
            _exploded = false;
            base.EndProjectile();
        }

        public override void OnGameEnd()
        {
            StopAllCoroutines();
            EndProjectile();
        }
    }
}