using System;
using GameObjectComponent.App;
using UnityEngine;

public class Gold : PersistentComponent
{
    [field:SerializeField] public int amount { get; private set; } = 0;
    [field:SerializeField] public int totalEarned { get; private set; } = 0;
    public Action<int> OnGoldChanged;

    public void ChangeGold(int goldDelta)
    {
        amount += goldDelta;
        amount = Mathf.Max(amount, 0);
        Save();
        OnGoldChanged?.Invoke(amount);
        
        if(goldDelta > 0)
            totalEarned += goldDelta;
    }
    
    public override void Save()
    {
        PlayerPrefs.SetInt($"gold_{id}", amount);
        PlayerPrefs.SetInt($"totalEarned_{id}", totalEarned);
    }

    public override void Load()
    {
        amount = PlayerPrefs.GetInt($"gold_{id}", 0);
        totalEarned = PlayerPrefs.GetInt($"totalEarned_{id}", 0);
        OnGoldChanged?.Invoke(amount);
    }

    public override void Reset()
    {
        amount = 0;
        totalEarned = 0;
        Save();
        OnGoldChanged?.Invoke(amount);
    }
}
