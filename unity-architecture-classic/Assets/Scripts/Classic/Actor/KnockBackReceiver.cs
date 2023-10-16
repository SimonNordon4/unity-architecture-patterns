using System;
using UnityEngine;

namespace Classic.Actor
{
    public class KnockBackReceiver : ActorComponent
    {
        public event Action<Vector3> OnKnockBack;

        public void ApplyKnockBack(Vector3 knockBack)
        {
            OnKnockBack?.Invoke(knockBack);
        }
    }
}