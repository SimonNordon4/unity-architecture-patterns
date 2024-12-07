using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIActorHealthBar : MonoBehaviour
    {
        [SerializeField]private Health health;
        [SerializeField]private Image fillBar;
        [SerializeField]private GameObject healthBar;
        [SerializeField] private UserSettings userSettings;


        private void ToggleHealthBar(bool show)
        {
            healthBar.SetActive(show);
        }

        private void OnEnable()
        {
            if (userSettings != null)
            {
                healthBar.SetActive(userSettings.ShowEnemyHealthBars);
            }
            userSettings.showHealthBarChanged.AddListener(ToggleHealthBar);
            health.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(health.currentHealth);
        }

        
        private void OnDisable()
        {
            health.OnHealthChanged.RemoveListener(UpdateHealthBar);
            userSettings.showHealthBarChanged.RemoveListener(ToggleHealthBar);
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