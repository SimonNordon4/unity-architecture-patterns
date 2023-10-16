using Classic.Game;
using UnityEngine;

namespace Classic.Actor
{
    public class ActorMovement : ActorComponent
    {
        public Vector3 velocity { get; set; }
        public Vector3 lookDirection { get; set; }

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void LateUpdate()
        {
            var newPosition = Vector3.zero;
            var newRotation = Quaternion.identity;

            if (velocity.magnitude > 0.01f)
            {
                _transform.position += velocity * GameTime.deltaTime;    
            }

            if (lookDirection.magnitude > 0.01f)
            {
                _transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        public override void Reset()
        {
            _transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}