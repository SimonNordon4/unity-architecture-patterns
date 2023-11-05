using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    /// <summary>
    /// Keeps track of how much Gold the player has, for purchasing upgrades.
    /// </summary>
    public class Gold : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [field:SerializeField] public int amount { get; private set; } = 0;
        [field:SerializeField] public int totalEarned { get; private set; } = 0;
        public event Action<int> OnGoldChanged;

        private void OnEnable()
        {
            state.OnGameLost += AddGameWhenGoldEnds;
            state.OnGameWon += AddGameWhenGoldEnds;
            Load();
        }

        private void OnDisable()
        {
            state.OnGameLost -= AddGameWhenGoldEnds;
            state.OnGameWon -= AddGameWhenGoldEnds;
            Save();
        }

        public void AddGold(int difference)
        {
            amount += difference;
            Save();
            OnGoldChanged?.Invoke(amount);
            totalEarned += difference;
        }

        public void RemoveGold(int difference)
        {
            amount -= difference;
            Save();
            OnGoldChanged?.Invoke(this.amount);
        }

        public void SetGold(int newAmount)
        {
            amount = newAmount;
            Save();
            OnGoldChanged?.Invoke(this.amount);
        }
        
        public void Save()
        {
            PlayerPrefs.SetInt("currentGold", amount);
            PlayerPrefs.SetInt("totalGold",totalEarned);
        }

        public void Load()
        {
            amount = PlayerPrefs.GetInt("currentGold", 0);
            Debug.Log("Loaded gold: " + amount);
            totalEarned = PlayerPrefs.GetInt("totalGold", 0);
            OnGoldChanged?.Invoke(amount);
        }

        public void Reset()
        {
            amount = 0;
            totalEarned = 0;
            Save();
            OnGoldChanged?.Invoke(amount);
        }

        public void AddGameWhenGoldEnds()
        {
            // // get the enemy manager
            // var enemyManager = FindObjectOfType<EnemyManager>();
            // var totalGold = Mathf.RoundToInt(enemyManager.WaveDatas.Sum(data => data.currentGold + data.bonusGold));
            // TODO: Calculate total gold.
            AddGold(1);
        }
    }
}