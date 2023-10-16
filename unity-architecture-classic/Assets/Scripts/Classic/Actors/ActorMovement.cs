using Classic.Game;
using UnityEngine;

namespace Classic.Actors
{
    public class ActorMovement : ActorComponent
    {
        [SerializeField] private Level level;

        public void Construct(Level newLevel)
        {
            level = newLevel;
        }
        
        public Vector3 velocity { get; set; }
        public Vector3 lookDirection { get; set; }

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void LateUpdate()
        {
            var position = _transform.position;
            
            // clamp newPosition to level bounds
            var x = Mathf.Clamp(position.x, -level.bounds.x, level.bounds.x);
            var y = Mathf.Clamp(position.z, -level.bounds.y, level.bounds.y);
            
            var newPosition = new Vector3(x, position.y, y);

            if (velocity.magnitude > 0.01f)
            {
                _transform.position = newPosition + velocity * GameTime.deltaTime;    
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