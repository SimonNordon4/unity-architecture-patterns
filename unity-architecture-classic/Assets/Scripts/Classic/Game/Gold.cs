using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Classic.Game
{
    public class Gold : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [field:SerializeField] public int amount { get; private set; } = 0;
        [field:SerializeField] public int totalEarned { get; private set; } = 0;
        public UnityEvent<int> onGoldChanged = new();

        private void OnEnable()
        {
            state.onGameLost.AddListener(AddGameWhenGoldEnds);
            state.onGameWon.AddListener(AddGameWhenGoldEnds);
            Load();
        }

        private void OnDisable()
        {
            state.onGameLost.RemoveListener(AddGameWhenGoldEnds);
            state.onGameWon.RemoveListener(AddGameWhenGoldEnds);
            Save();
        }

        public void AddGold(int difference)
        {
            amount += difference;
            Save();
            onGoldChanged.Invoke(amount);
            totalEarned += difference;
        }

        public void RemoveGold(int difference)
        {
            amount -= difference;
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
            PlayerPrefs.SetInt("currentGold", amount);
            PlayerPrefs.SetInt("totalGold",totalEarned);
        }

        public void Load()
        {
            amount = PlayerPrefs.GetInt("currentGold", 0);
            totalEarned = PlayerPrefs.GetInt("totalGold", 0);
            onGoldChanged.Invoke(amount);
        }

        public void Reset()
        {
            amount = 0;
            totalEarned = 0;
            Save();
            onGoldChanged.Invoke(amount);
        }

        public void AddGameWhenGoldEnds()
        {
            // get the enemy manager
            var enemyManager = FindObjectOfType<EnemyManager>();

            var totalGold = Mathf.RoundToInt(enemyManager.WaveDatas.Sum(data => data.currentGold + data.bonusGold));

            AddGold(totalGold);
        }
    }
}