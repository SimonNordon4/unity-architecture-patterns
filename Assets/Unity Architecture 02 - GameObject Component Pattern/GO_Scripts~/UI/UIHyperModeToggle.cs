using GameObjectComponent.App;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIHyperModeToggle : MonoBehaviour
    {
        [SerializeField] private Settings settings;
        [SerializeField] private Toggle hyperModeToggle;
        
        private void OnEnable()
        {
            hyperModeToggle.isOn = settings.hyperMode;
            hyperModeToggle.onValueChanged.AddListener((value) => settings.SetHyperMode(value));
        }
        
        private void OnDestroy()
        {
            hyperModeToggle.onValueChanged.RemoveAllListeners();
        }
    }
}