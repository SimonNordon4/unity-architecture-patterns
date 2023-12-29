using GameplayComponents.Locomotion;
using UnityEngine;

namespace GameObjectComponent.Debugging
{
    [RequireComponent(typeof(KnockBackReceiver))]
    public class KnockBackDebug : DebugComponent
    {
        private KnockBackReceiver _knockBackReceiver;

        private void OnEnable()
        {
            _knockBackReceiver = GetComponent<KnockBackReceiver>();
            _knockBackReceiver.OnKnockBack += OnKnockBack;
        }

        private void OnKnockBack(Vector3 obj)
        {
            Debug.Log($"KnockBack Force: {obj.magnitude:F2}");
            Debug.DrawRay(transform.position, obj, Color.red, 1f);
        }

        private void OnDisable()
        {
            _knockBackReceiver.OnKnockBack -= OnKnockBack;
        }
    }
}