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
        
        public Vector3 velocity { get; private set; }
        public Vector3 lookDirection { get; private set; }
        
        public float acceleration { get; private set; }

        private Transform _transform;
        private Vector3 _initialPosition;

        private void Awake()
        {
            _transform = transform;
            _initialPosition = _transform.position;
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

            velocity = Vector3.zero;
        }
        public override void Reset()
        {
            _transform.SetPositionAndRotation(_initialPosition, Quaternion.identity);
        }

        public void SetVelocity(Vector3 newVelocity)
        {
            velocity = newVelocity;
        }

        public void SetLookDirection(Vector3 newDirection)
        {
            lookDirection = newDirection;
        }

        public void SetVelocityAndLookDirection(Vector3 combinedVelocity)
        {
            velocity = combinedVelocity;
            lookDirection = combinedVelocity.normalized;
        }
        
        public void SetVelocityAndLookDirection(Vector3 newVelocity, Vector3 newLookDirection)
        {
            velocity = newVelocity;
            lookDirection = newLookDirection;
        }
        
        public void AddVelocity(Vector3 newVelocity)
        {
            velocity += newVelocity;
        }
        
        public void SetAcceleration(float newAcceleration)
        {
            acceleration = newAcceleration;
        }
    }
}