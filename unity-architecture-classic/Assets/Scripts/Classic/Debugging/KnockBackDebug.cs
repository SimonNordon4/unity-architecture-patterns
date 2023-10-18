using Classic.Actors;
using UnityEngine;

namespace Classic.Debugging
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
            Debug.Log($"KnockBack: {obj}");
            Debug.DrawRay(transform.position, obj, Color.red, 1f);
        }

        private void OnDisable()
        {
            _knockBackReceiver.OnKnockBack -= OnKnockBack;
        }
    }
}