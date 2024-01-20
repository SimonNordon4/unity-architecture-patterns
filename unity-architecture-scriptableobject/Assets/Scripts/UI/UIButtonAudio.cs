using GameObjectComponent.App;
using GameObjectComponent.Definitions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button _button;
    private SoundManager _soundManager;
    
    [SerializeField] private SoundDefinition hoverSound;
    [SerializeField] private SoundDefinition clickSound;
    
    
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button != null && !_button.interactable || _soundManager == null) return;
        _soundManager.PlaySound(hoverSound);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_button != null && !_button.interactable || _soundManager == null) return;
        _soundManager.PlaySound(clickSound);
    }
}
