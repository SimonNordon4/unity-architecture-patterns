using UnityEngine;

    public class Chest : MonoBehaviour
    {
        public int minTier = 1;
        public int maxTier = 1;
        public ChestType chestType = ChestType.Mini;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
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
