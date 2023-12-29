using System.Collections.Generic;
using GameObjectComponent.GameplayComponents.Life;
using GameObjectComponent.Pools;
using GameplayComponents.Actor;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIActorPoolHealthBars : MonoBehaviour
    {
        [SerializeField]private ActorPool _actorPool;
        [SerializeField]private UITextPool textPool;
        [SerializeField]private Camera gameCamera;
        private readonly Dictionary<Health, TextMeshProUGUI> _healthBars = new();
        
        private void OnEnable()
        {
            _actorPool.OnEnemySpawned += OnActorSpawned;
            _actorPool.OnEnemyDespawned += OnActorDespawned;    
        }

        private void OnActorSpawned(PoolableActor actor)
        {
            var health = actor.GetComponent<Health>();
            var healthBar = textPool.GetText();
            _healthBars.Add(health, healthBar);
            healthBar.transform.position = actor.transform.position;
        }
        
        private void OnActorDespawned(PoolableActor actor)
        {
            var health = actor.GetComponent<Health>();
            var healthBar = _healthBars[health];
            textPool.ReturnDamageNumber(healthBar);
            _healthBars.Remove(health);
        }

        private void OnDisable()
        {
            _actorPool.OnEnemySpawned -= OnActorSpawned;
            _actorPool.OnEnemyDespawned -= OnActorDespawned;    
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