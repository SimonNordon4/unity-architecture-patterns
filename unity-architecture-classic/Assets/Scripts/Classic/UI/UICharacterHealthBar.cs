using Classic.Actors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Classic.UI
{
    public class UICharacterHealthBar : MonoBehaviour
    {
        [SerializeField]private ActorHealth health;
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