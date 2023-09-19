using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SettingsManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    public bool showDamageNumbers = true;
    public bool showEnemyHealthBars = true;
    public bool isNormalSpawnRate = true;
    
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    
    public void SetShowEnemyHealthBars(bool show)
    {
        showEnemyHealthBars = show;
        
        // get every enemy in the scene
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.SetHealthBarVisibility(showEnemyHealthBars);
        }
    }
    
    public void SetNormalSpawnRate(bool normal)
    {
        isNormalSpawnRate = normal;
    }
    
    public void ShowDamageNumbers(bool show)
    {
        showDamageNumbers = show;
    }

    public void ToggleTimeScale()
    {
        if(Time.timeScale - 1 < 0.01f)
            Time.timeScale = 2f;
        else
            Time.timeScale = 1f;
    }
}
