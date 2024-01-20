using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class Movement : GameplayComponent
    {
        [field: SerializeField] public bool canMove { get;  set; } = true;
        [field:SerializeField] public Level level { get; private set; }

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
            if(!canMove) return;
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

        public override void OnGameStart()
        {
            _transform.position = _initialPosition;
            _transform.rotation = Quaternion.identity;
        }
        
        public override void OnGameEnd()
        {
            velocity = Vector3.zero;
            acceleration = 0;
        }
    }
}