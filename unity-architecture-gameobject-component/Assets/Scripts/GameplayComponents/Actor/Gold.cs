using System;
using GameObjectComponent.App;
using UnityEngine;

public class Gold : PersistentComponent
{
    [field:SerializeField] public int amount { get; private set; } = 0;
    public Action<int> OnGoldChanged;

    public void AddGold(int addedGold)
    {
        Debug.Log($"Adding {addedGold} gold to {name}");
        amount += addedGold;
        Save();
        OnGoldChanged?.Invoke(amount);
    }
    
    public override void Save()
    {
        PlayerPrefs.SetInt($"gold_{id}", amount);
    }

    public override void Load()
    {
        amount = PlayerPrefs.GetInt($"gold_{id}", 0);
        OnGoldChanged?.Invoke(amount);
    }
}
