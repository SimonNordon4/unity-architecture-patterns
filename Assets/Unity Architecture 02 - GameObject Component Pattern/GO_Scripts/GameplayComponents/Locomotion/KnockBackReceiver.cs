using System;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class KnockBackReceiver : GameplayComponent
    {
        public event Action<Vector3> OnKnockBack;

        public void ApplyKnockBack(Vector3 knockBack)
        {
            OnKnockBack?.Invoke(knockBack);
        }
    }
}