using System;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class KnockBackReceiver : MonoBehaviour
    {
        public event Action<Vector3> OnKnockBack;

        public void ApplyKnockBack(Vector3 knockBack)
        {
            OnKnockBack?.Invoke(knockBack);
        }
    }
}