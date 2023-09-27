using UnityEngine;
using UnityEngine.Events;

namespace Classic.Core
{
    public class Chest : MonoBehaviour
    {
        public static readonly UnityEvent<Chest> OnChestPickedUp = new();
        public UnityEvent onPickedUp = new();
        
        [SerializeField]private LayerMask playerLayer;
        
        [field:SerializeField] public Vector2Int tiers { get; private set; }= new(1, 5);
        [field:SerializeField] public Vector2Int options { get; private set; } = new(2, 5);
        [field:SerializeField] public ChestType chestType { get; private set; } = ChestType.Mini;

        [field:SerializeField] public int numberOfItems { get; set; }
        [field:SerializeField] public ChestItem[] chestItems { get; set; }

        public void Construct(ChestDefinition def)
        {
            tiers = def.tiers;
            options = def.options;
            chestType = def.chestType;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerLayer != (playerLayer | (1 << other.gameObject.layer))) return;
            OnChestPickedUp.Invoke(this);
            Destroy(gameObject);
        }
    }
}