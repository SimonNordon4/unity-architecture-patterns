using GameObjectComponent.Game;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents.Locomotion
{
    public class MoveTowardsRandomLocation : GameplayComponent
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Stats stat;
        
        private Stat _speedStat;
        
        private float _distanceToTarget;
        private Vector3 _randomLocation;

        private void Start()
        {
            _speedStat = stat.GetStat(StatType.MoveSpeed);
            _randomLocation = GetRandomLocation();
        }

        private void Update()
        {
            // move towards random location
            var position = transform.position;
            var direction = (_randomLocation - position).normalized;
            var speed = _speedStat.value;
            var velocity = direction * speed;
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
            var bounds = movement.level.bounds;
            
            var randomX = Random.Range(-bounds.x, bounds.x);
            var randomZ = Random.Range(-bounds.y, bounds.y);
            
            return new Vector3(randomX, 0, randomZ);
        }
    }
}