using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button != null && !_button.interactable) return;
            AudioManager.Instance.ButtonHover();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_button != null && !_button.interactable) return;
            AudioManager.Instance.ButtonClick();
        }
    }
}
