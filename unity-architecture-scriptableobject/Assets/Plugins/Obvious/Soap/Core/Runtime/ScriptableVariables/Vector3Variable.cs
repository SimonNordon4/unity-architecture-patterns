using UnityEngine;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_variable_vector3.asset", menuName = "Soap/ScriptableVariables/vector3")]
    public class Vector3Variable : ScriptableVariable<Vector3>
    {
        public override void Save()
        {
            PlayerPrefs.SetFloat(Guid + "_x", Value.x);
            PlayerPrefs.SetFloat(Guid + "_y", Value.y);
            PlayerPrefs.SetFloat(Guid + "_z", Value.z);
            base.Save();
        }

        public override void Load()
        {
            var x = PlayerPrefs.GetFloat(Guid + "_x", DefaultValue.x);
            var y = PlayerPrefs.GetFloat(Guid + "_y", DefaultValue.y);
            var z = PlayerPrefs.GetFloat(Guid + "_z", DefaultValue.z);
            Value = new Vector3(x,y,z);
            base.Load();
        }
    }
}