using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class TransformData : ScriptableObject
    {
        public bool IsNull { get; set; }
        public Transform Data { get;  set; }
    }
}



