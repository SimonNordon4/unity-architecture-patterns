using GameObjectComponent.GameplayComponents.Combat;
using GameObjectComponent.GameplayComponents.Life;
using UnityEngine;

namespace GameObjectComponent.Dummy
{
    [RequireComponent(typeof(DamageReceiver))]
    public class DummyHealth : MonoBehaviour
    {
        [SerializeField]private int health = 10;
        private DamageReceiver _damageReceiver;
        
        private void Awake()
        {
            _damageReceiver = GetComponent<DamageReceiver>();
        }
        
        private void OnEnable()
        {
            _damageReceiver.OnDamageReceived += OnDamage;
        }
        
        private void OnDamage(int obj)
        {
            health -= obj;
            Debug.Log($"Health: {health}");
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDisable()
        {
            _damageReceiver.OnDamageReceived -= OnDamage;
        }
    }
}