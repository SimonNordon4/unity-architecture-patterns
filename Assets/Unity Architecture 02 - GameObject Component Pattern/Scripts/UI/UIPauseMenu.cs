using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIPauseMenu : MonoBehaviour
    {
        public PlayerController playerController;
        
        [Header("Buttons")]
        public Button resume;
        public Button restart;
        public Button settings;
        public Button quit;
        
        [Header("Stats")]
        public RectTransform statContainer;
        
        public TextMeshProUGUI textDescriptionPrefab;

        public List<TextMeshProUGUI> textDescriptions = new();

        [Header("Items")]
        public RectTransform itemContainer;
        public UIChestItemHoverImage chestItemHoverImagePrefab;
        public List<UIChestItemHoverImage> chestItemHoverImages = new();
        
        
        
        private void OnEnable()
        {
            foreach (TextMeshProUGUI textDescription in textDescriptions)
            {
                Destroy(textDescription.gameObject);
            }
            
            textDescriptions.Clear();
            PopulateStats();

            foreach (UIChestItemHoverImage chestItemHoverImage in chestItemHoverImages)
            {
                Destroy(chestItemHoverImage.gameObject);
            }
            chestItemHoverImages.Clear();
            
            PopulateItems();
            
            resume.onClick.AddListener(GameManager.Instance.TogglePauseGame);
            
            restart.onClick.AddListener(ResetClicked);
            
            settings.onClick.AddListener(GameManager.Instance.ShowSettingsMenu);
            quit.onClick.AddListener(GameManager.Instance.LoadMainMenu);
        }

        private void OnDisable()
        {
            resume.onClick.RemoveAllListeners();
            restart.onClick.RemoveAllListeners();
            settings.onClick.RemoveAllListeners();
            quit.onClick.RemoveAllListeners();
        }

        public void ResetClicked()
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.StartNewGame();
            gameObject.SetActive(false);
        }

        private void PopulateStats()
        {
            var healthText = Instantiate(textDescriptionPrefab, statContainer);
            healthText.text = "Max HP: " + playerController.playerMaxHealth.value;
            
            var healthRegenText = Instantiate(textDescriptionPrefab, statContainer);
            healthRegenText.text = "HP Regen: " + playerController.playerMaxHealth.value;
            
            var speedText = Instantiate(textDescriptionPrefab, statContainer);
            speedText.text = "Speed: " + playerController.playerSpeed.value;
            
            var armorText = Instantiate(textDescriptionPrefab, statContainer);
            armorText.text = "Armor: " + playerController.armor.value;
            
            var dodgeText = Instantiate(textDescriptionPrefab, statContainer);
            dodgeText.text = "Dodge: " + playerController.dodge.value;
            
            var damageText = Instantiate(textDescriptionPrefab, statContainer);
            damageText.text = "Damage: " + playerController.damage.value;
            
            var fireRateText = Instantiate(textDescriptionPrefab, statContainer);
            fireRateText.text = "FireRate: " + playerController.firerate.value;
            
            var critChance = Instantiate(textDescriptionPrefab, statContainer);
            critChance.text = "Crit Chance: " + playerController.critChance.value;
            
            var rangeText = Instantiate(textDescriptionPrefab, statContainer);
            rangeText.text = "Range: " + playerController.range.value;
            
            var knockbackText = Instantiate(textDescriptionPrefab, statContainer);
            knockbackText.text = "Knockback: " + playerController.knockback.value;
            
            var pierceText = Instantiate(textDescriptionPrefab, statContainer);
            pierceText.text = "Pierce: " + playerController.pierce.value;
            
            textDescriptions.Add(healthText);
            textDescriptions.Add(healthRegenText);
            textDescriptions.Add(speedText);
            textDescriptions.Add(armorText);
            textDescriptions.Add(dodgeText);
            textDescriptions.Add(damageText);
            textDescriptions.Add(fireRateText);
            textDescriptions.Add(critChance);
            textDescriptions.Add(rangeText);
            textDescriptions.Add(knockbackText);
            textDescriptions.Add(pierceText);
        }

        public void PopulateItems()
        {
            var currentItems = playerController.currentlyHeldItems;
            var itemCounts = new Dictionary<ChestItem, int>();

            foreach (var item in currentItems)
            {
                if (itemCounts.ContainsKey(item))
                {
                    itemCounts[item]++;
                }
                else
                {
                    itemCounts[item] = 1;
                }
            }

            foreach (var kvp in itemCounts)
            {
                var hoverImage = Instantiate(chestItemHoverImagePrefab, itemContainer);
                hoverImage.Initialize(kvp.Key, kvp.Value);
                chestItemHoverImages.Add(hoverImage);
            }
        }
    }
}
