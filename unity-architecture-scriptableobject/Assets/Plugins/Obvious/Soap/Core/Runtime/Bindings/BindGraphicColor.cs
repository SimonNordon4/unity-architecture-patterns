using UnityEngine;
using UnityEngine.UI;

namespace Obvious.Soap
{
    /// <summary>
    /// Binds a color variable to a graphic (works with UI and SpriteRenderer)
    /// </summary>
    [AddComponentMenu("Soap/Bindings/BindGraphicColor")]
    [RequireComponent(typeof(Graphic))]
    public class BindGraphicColor : CacheComponent<Graphic>
    {
        [SerializeField] private ColorVariable _colorVariable = null;
      
        protected override void Awake()
        {
            base.Awake();
            Refresh(_colorVariable);
            _colorVariable.OnValueChanged += Refresh;
        }

        private void OnDestroy()
        {
            _colorVariable.OnValueChanged -= Refresh;
        }

        private void Refresh(Color color)
        {
            _component.color = color;
        }
    }
}