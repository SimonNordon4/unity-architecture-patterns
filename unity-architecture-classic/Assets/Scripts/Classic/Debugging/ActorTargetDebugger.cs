using Classic.Actors;
using UnityEngine;

namespace Classic.Debugging
{
    public class ActorTargetDebugger : DebugComponent
    {
        [SerializeField] private ActorTarget target;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float range = 10f;

        private void Update()
        {
            if (target.hasTarget)
            {
                Debug.DrawLine(target.transform.position, target.target.position, Color.red);
            }
        }
        
        [ContextMenu("Get Target")]
        public void GetTarget()
        {
            target.GetClosestTarget(range);
        }
    }
}