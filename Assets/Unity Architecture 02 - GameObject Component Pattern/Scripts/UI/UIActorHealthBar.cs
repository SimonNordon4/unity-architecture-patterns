using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIActorHealthBar : MonoBehaviour
    {
        [SerializeField]private Health health;
        [SerializeField]private Image fillBar;

        private void OnEnable()
        {
            health.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(health.currentHealth);
        }

        
        private void OnDisable()
        {
            health.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;           
        }

        

        private void UpdateHealthBar(int currentHealth)
        {  
            fillBar.fillAmount = currentHealth / (float)health.maxHealth;
        }
    }
}