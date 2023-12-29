using System;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameplayComponents.Items
{
    public class Pickup : GameplayComponent
    {
        public event Action OnPickedUp;
        public event Action OnExpired;

        [SerializeField] private LayerMask pickupLayer;
        [SerializeField] private float lifeTime = 1f;
        
        private float _timeAlive;
        
        public void Update()
        {
            _timeAlive += GameTime.deltaTime;
            if (!(_timeAlive >= lifeTime)) return;
            OnExpired?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(pickupLayer != (pickupLayer | (1 << other.gameObject.layer))) return;
            OnPickedUp?.Invoke();
        }

        private void Consume()
        {
            Destroy(this);
        }
    }

    
}