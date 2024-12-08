using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class TransformData : ScriptableData
    {
        public bool IsNull { get; set; }
        public Transform Data { get;  set; }
        
        public override void ResetData()
        {
            Data = null;
            IsNull = true;
        }
    }
}



