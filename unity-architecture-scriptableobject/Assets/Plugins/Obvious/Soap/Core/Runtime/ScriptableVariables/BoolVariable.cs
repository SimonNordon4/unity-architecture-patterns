using UnityEngine;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_variable_bool.asset", menuName = "Soap/ScriptableVariables/bool")]
    [System.Serializable]
    public class BoolVariable : ScriptableVariable<bool>
    {
        public override void Save()
        {
            PlayerPrefs.SetInt(Guid, Value ? 1 : 0);
            base.Save();
        }

        public override void Load()
        {
            Value = PlayerPrefs.GetInt(Guid, DefaultValue ? 1 : 0) > 0;
            base.Load();
        }

        /// <summary>
        /// Use this to toggle the value of the variable.
        /// </summary>
        public void Toggle()
        {
            Value = !Value;
        }
    }
}