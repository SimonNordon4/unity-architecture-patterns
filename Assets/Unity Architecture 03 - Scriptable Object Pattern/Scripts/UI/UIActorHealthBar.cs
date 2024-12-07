using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIActorHealthBar : MonoBehaviour
    {
        [SerializeField]private Health health;
        [SerializeField]private Image fillBar;
        [SerializeField]private GameObject healthBar;
        
        private UserSettings _userSettings;

        public void Construct(UserSettings userSettings)
        {
            _userSettings = userSettings;
            _userSettings.showHealthBarChanged.AddListener(ToggleHealthBar);
        }

        private void ToggleHealthBar(bool show)
        {
            healthBar.SetActive(show);
        }

        private void OnEnable()
        {
            if (_userSettings != null)
            {
                healthBar.SetActive(_userSettings.ShowEnemyHealthBars);
            }
            
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