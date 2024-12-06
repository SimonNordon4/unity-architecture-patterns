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
            if (_regenTimer >= _healthRegenStat.value * 0.5f)
            {
                _health.AddHealth(1);
                _regenTimer = 0f;
            }
        }
    }
}