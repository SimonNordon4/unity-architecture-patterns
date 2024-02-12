using UnityEngine;
using Obvious.Soap.Attributes;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_variable_int.asset", menuName = "Soap/ScriptableVariables/int")]
    public class IntVariable : ScriptableVariable<int>
    {
        [Tooltip("Clamps the value of this variable to a minimum and maximum.")] 
        [SerializeField] private bool _isClamped = false;
        public bool IsClamped => _isClamped;

        [Tooltip("If clamped, sets the minimum and maximum")] [SerializeField]
        [ShowIf("_isClamped", true)]
        private Vector2Int _minMax = new Vector2Int(0, int.MaxValue);
        
        public Vector2Int MinMax { get => _minMax; set => _minMax = value;}
        public int Min { get => _minMax.x; set => _minMax.x = value;}
        public int Max { get => _minMax.y; set => _minMax.y = value;}
        
        /// <summary>
        /// Returns the percentage of the value between the minimum and maximum.
        /// </summary>
        public float Ratio => Mathf.InverseLerp(_minMax.x, _minMax.y, _value);

        public override void Save()
        {
            PlayerPrefs.SetInt(Guid, Value);
            base.Save();
        }

        public override void Load()
        {
            Value = PlayerPrefs.GetInt(Guid, DefaultValue);
            base.Load();
        }

        public void Add(int value)
        {
            Value += value;
        }

        public override int Value
        {
            get => _value;
            set
            {
                var clampedValue = IsClamped ? Mathf.Clamp(value, _minMax.x, _minMax.y) : value;
                base.Value = clampedValue;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (IsClamped)
            {
                var clampedValue = Mathf.Clamp(_value, _minMax.x, _minMax.y);
                if (_value < clampedValue || _value > clampedValue)
                    _value = clampedValue;
            }

            base.OnValidate();
        }
#endif
    }
}