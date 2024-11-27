using System;
using GameObjectComponent.Definitions;
using GameplayComponents;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectComponent.Items
{
    public enum ChestType
    {
        Mini,
        Medium,
        Large,
    }
    
    public class Chest : GameplayComponent
    {
        public static Action<Chest> OnChestPickedUp;
        public UnityEvent onPickedUp = new();
        
        [SerializeField]private LayerMask playerLayer;
        
        [field:SerializeField] public Vector2Int tiers { get; private set; }= new(1, 5);
        [field:SerializeField] public Vector2Int options { get; private set; } = new(2, 5);
        [field:SerializeField] public ChestType chestType { get; private set; } = ChestType.Mini;

        [field:SerializeField] public int numberOfItems { get; set; }
        [field:SerializeField] public ChestItemDefinition[] chestItems { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (playerLayer != (playerLayer | (1 << other.gameObject.layer))) return;
            OnChestPickedUp.Invoke(this);
            onPickedUp.Invoke();
        }

        public override void OnGameEnd()
        {
            Destroy(this.gameObject);
        }
    }
}