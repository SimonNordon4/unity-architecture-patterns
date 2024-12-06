using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIPlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image fillBar;
        [SerializeField]private TextMeshProUGUI healthBarText;

        private void OnEnable()
        {
            health.OnHealthChanged.AddListener(UpdateHealthBar);
        }

        private void OnDisable()
        {
            health.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }

        private void UpdateHealthBar(int currentHealth)
        {
            fillBar.fillAmount = currentHealth / (float)health.maxHealth;
            healthBarText.text = $"{currentHealth}/{health.maxHealth}";
        }
    }
}
