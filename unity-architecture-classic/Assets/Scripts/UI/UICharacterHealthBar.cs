using Classic.Character;
using Classic.Game;
using TMPro;

namespace UnityEngine.UI
{
    public class UICharacterHealthBar : MonoBehaviour
    {
        [SerializeField]private Stats stats;
        [SerializeField]private CharacterHealth health;
        [SerializeField]private Image healthBar;
        [SerializeField]private TextMeshProUGUI healthText;

        private void OnEnable()
        {
            health.onHealthChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDisable()
        {
            health.onHealthChanged -= UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            healthText.text = $"{health.currentHealth} / {stats.playerHealth.value}";
            healthBar.fillAmount = (float)health.currentHealth / stats.playerHealth.value;
        }
    }
}