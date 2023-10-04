using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    public class Gold : MonoBehaviour
    {
        [field:SerializeField] public int amount { get; private set; } = 0;
        public UnityEvent<int> onGoldChanged = new();

        public void AddGold(int difference)
        {
            this.amount += difference;
            Save();
            onGoldChanged.Invoke(this.amount);
        }

        public void RemoveGold(int difference)
        {
            this.amount -= difference;
            Save();
            onGoldChanged.Invoke(this.amount);
        }

        public void SetGold(int newAmount)
        {
            amount = newAmount;
            Save();
            onGoldChanged.Invoke(this.amount);
        }
        
        public void Save()
        {
            PlayerPrefs.SetInt("gold", amount);
        }

        public void Load()
        {
            amount = PlayerPrefs.GetInt("gold", 0);
            onGoldChanged.Invoke(amount);
        }

        public void Reset()
        {
            amount = 0;
            Save();
            onGoldChanged.Invoke(amount);
        }
    }
}