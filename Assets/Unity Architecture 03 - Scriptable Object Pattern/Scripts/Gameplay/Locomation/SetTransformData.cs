using UnityEngine;


namespace UnityArchitecture.ScriptableObjectPattern
{
    [DefaultExecutionOrder(-15)]
    public class SetTransformData : MonoBehaviour
    {
        [SerializeField] private TransformData targetTransform;

        void Awake()
        {
            targetTransform.Data = transform;
            targetTransform.IsNull = false;
        }

        private void OnDestroy()
        {
            targetTransform.IsNull = true;
        }
    }
}

