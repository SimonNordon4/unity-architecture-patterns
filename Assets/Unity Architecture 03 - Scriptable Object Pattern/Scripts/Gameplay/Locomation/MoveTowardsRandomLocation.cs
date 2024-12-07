using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
   
    public class MoveTowardsRandomLocation : MoveBase
    {

        
        private float _distanceToTarget;
        private Vector3 _randomLocation;

        private void Start()
        {
            movement = GetComponent<Movement>();
            speedStat = GetComponent<Stats>().GetStat(StatType.Speed);
            _randomLocation = GetRandomLocation();
        }

        private void Update()
        {
            // move towards random location
            var position = transform.position;
            var direction = (_randomLocation - position).normalized;
            var velocity = direction * Speed;
            movement.SetVelocity(velocity);
            movement.SetLookDirection(direction);
            
            // check if we've reached the target
            _distanceToTarget = Vector3.Distance(position, _randomLocation);
            if (_distanceToTarget < 0.5f)
            {
                _randomLocation = GetRandomLocation();
            }
        }
        
        private Vector3 GetRandomLocation()
        {
            var bounds = movement.Level.Bounds;
            
            var randomX = Random.Range(-bounds.x, bounds.x);
            var randomZ = Random.Range(-bounds.y, bounds.y);
            
            return new Vector3(randomX, 0, randomZ);
        }
    }
}