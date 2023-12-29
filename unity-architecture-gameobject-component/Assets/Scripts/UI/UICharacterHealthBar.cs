using GameObjectComponent.GameplayComponents.Combat;
using GameObjectComponent.GameplayComponents.Life;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UICharacterHealthBar : MonoBehaviour
    {
        [SerializeField]private Health health;
        [SerializeField]private Image healthBar;
        [SerializeField]private TextMeshProUGUI healthText;

        private void OnEnable()
        {
            health.OnHealthChanged += UpdateUI;
            UpdateUI(health.currentHealth);
        }

        private void UpdateUI(int currentHealth)
        {            
            healthText.text = $"{currentHealth} / {health.maxHealth}";
            healthBar.fillAmount = (float)currentHealth / health.maxHealth;       
        }

        private void OnDisable()
        {
            health.OnHealthChanged -= UpdateUI;
            UpdateUI(health.currentHealth);
        }
    }
}