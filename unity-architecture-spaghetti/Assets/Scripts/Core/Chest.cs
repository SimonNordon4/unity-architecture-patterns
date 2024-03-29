﻿using UnityEngine;

    public class Chest : MonoBehaviour
    {
        public int minTier = 1;
        public int maxTier = 1;
        public Vector2Int options = new(0, 0);
        public ChestType chestType = ChestType.Mini;
        
        public SoundDefinition openSound;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                AudioManager.instance.PlaySound(openSound);
                GameManager.instance.PickupChest(this);
                
                Destroy(gameObject);
            }
        }
    }

public enum ChestType
{
    Mini,
    Medium,
    Large
}
