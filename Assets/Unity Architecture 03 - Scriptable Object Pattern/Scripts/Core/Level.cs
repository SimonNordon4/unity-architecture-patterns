using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class Level : MonoBehaviour
    {
        [field:SerializeField]
        public Vector2 Bounds {get;private set;} = new(17.5f, 17.5f);
    }
}