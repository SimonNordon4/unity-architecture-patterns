using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class AccountManager : MonoBehaviour
    {
        private static AccountManager _instance;
        public static AccountManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<AccountManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        
        public int totalGold;

        public void AddGold(int amount)
        {
            totalGold += amount;
        }

        private void OnEnable()
        {
            Load();
        }
        private void OnDisable()
        {
            Save();
        }


        public void Save()
        {
            var accountSave = new AccountSave();
            accountSave.totalGold = totalGold;
            
            var json = JsonUtility.ToJson(accountSave);
            PlayerPrefs.SetString("account", json);
        }
        
        public void Load()
        {
            var json = PlayerPrefs.GetString("account");
            if (string.IsNullOrEmpty(json))
            {
                totalGold = 0;
                return;
            }

            var accountSave = JsonUtility.FromJson<AccountSave>(json);
            totalGold = accountSave.totalGold;
        }
    }

    [Serializable]
    public struct AccountSave
    {
        public int totalGold;
    }
}