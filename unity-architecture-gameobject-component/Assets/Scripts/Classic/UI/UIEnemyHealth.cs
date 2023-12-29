using System.Collections.Generic;
using Classic.Actors;
using Classic.Enemies;
using TMPro;
using UnityEngine;

namespace Classic.UI
{
    public class UIEnemyHealth : MonoBehaviour
    {
        [SerializeField]private EnemyPool enemyPool;
        [SerializeField]private UITextPool textPool;
        [SerializeField]private Camera gameCamera;
        private readonly Dictionary<ActorHealth, TextMeshProUGUI> _healthBars = new();
        
        private void OnEnable()
        {
            enemyPool.OnEnemySpawned += OnEnemySpawned;
            enemyPool.OnEnemyDespawned += OnEnemyDespawned;    
        }

        private void OnEnemySpawned(PoolableEnemy enemy)
        {
            var health = enemy.GetComponent<ActorHealth>();
            var healthBar = textPool.GetText();
            _healthBars.Add(health, healthBar);
            healthBar.transform.position = enemy.transform.position;
        }
        
        private void OnEnemyDespawned(PoolableEnemy enemy)
        {
            var health = enemy.GetComponent<ActorHealth>();
            var healthBar = _healthBars[health];
            textPool.ReturnDamageNumber(healthBar);
            _healthBars.Remove(health);
        }

        private void OnDisable()
        {
            enemyPool.OnEnemySpawned -= OnEnemySpawned;
            enemyPool.OnEnemyDespawned -= OnEnemyDespawned;    
        }

        private void Update()
        {
            foreach (var healthBar in _healthBars)
            {
                // we need to convert the text to screen space from the enemies world space position
                var screenPosition = gameCamera.WorldToScreenPoint(healthBar.Key.transform.position);
                // offset the screen position so it's above their head
                screenPosition.y += 100;
                
                healthBar.Value.transform.position = screenPosition;
                var currentHealth = healthBar.Key.currentHealth;
                healthBar.Value.text = currentHealth > 0 ? currentHealth.ToString() : "";
            }
        }
    }
}