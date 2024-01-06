using System.Collections.Generic;
using GameObjectComponent.Pools;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using Pools;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIActorPoolHealthBars : MonoBehaviour
    {
        [SerializeField]private ActorPool actorPool;
        [SerializeField]private UITextPool textPool;
        [SerializeField]private Camera gameCamera;
        private readonly Dictionary<Health, TextMeshProUGUI> _healthBars = new();
        
        private void OnEnable()
        {
            actorPool.OnActorGet += OnActorGet;
            actorPool.OnActorReturn += OnActorReturn;    
        }

        private void OnActorGet(PoolableActor actor)
        {
            Debug.Log($"Actor {actor.name} was retrieved from the pool");
            if(actor.TryGetComponentDeep<Health>(out var health) == false)
                return;
            var healthBar = textPool.GetText();
            _healthBars.Add(health, healthBar);
            healthBar.transform.position = actor.transform.position;
        }
        
        private void OnActorReturn(PoolableActor actor)
        {
            if(actor.TryGetComponentDeep<Health>(out var health) == false)
                return;
            var healthBar = _healthBars[health];
            textPool.ReturnDamageNumber(healthBar);
            _healthBars.Remove(health);
        }

        private void OnDisable()
        {
            actorPool.OnActorGet -= OnActorGet;
            actorPool.OnActorReturn -= OnActorReturn;    
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