using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(Health))]
    public class HealthRegen : MonoBehaviour
    {
        private Stat _healthRegenStat;
        private Health _health;
        private float _regenTimer;

        private void Awake()
        {
            _healthRegenStat = GetComponent<Stats>().GetStat(StatType.HealthRegen);
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            _regenTimer += Time.deltaTime;
            // we need to add health as the value goes up.
            // 10 hp regen = 1 hp per 5 seconds.
            // 5 hp regen = 1 hp per 10 seconds.
            // 20 hp regen = 1 hp per 2.5 seconds. etc
            if (_regenTimer >= 50f / _healthRegenStat.value)
            {
                _regenTimer = 0;
                _health.AddHealth(1);
            }
            
           
        }
    }
}