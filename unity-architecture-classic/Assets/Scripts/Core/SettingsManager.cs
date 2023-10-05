using System.Collections;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle hyperModeToggle;
    public GameObject hyperText;

    private void Start()
    {
        hyperModeToggle.isOn = isHyperMode;
    }

    public bool showDamageNumbers = true;
    public bool showEnemyHealthBars = true;
    
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    
    public bool isHyperMode = false;

    private void OnEnable()
    {
        Load();
    }

    private void OnDisable()
    {
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetInt("showDamageNumbers", showDamageNumbers ? 1 : 0);
        PlayerPrefs.SetInt("showEnemyHealthBars", showEnemyHealthBars ? 1 : 0);
        PlayerPrefs.SetInt("isHyperMode", isHyperMode ? 1 : 0);
        
    }

    private void Load()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        showDamageNumbers = PlayerPrefs.GetInt("showDamageNumbers", 1) == 1;
        showEnemyHealthBars = PlayerPrefs.GetInt("showEnemyHealthBars", 1) == 1;
        isHyperMode = PlayerPrefs.GetInt("isHyperMode", 0) == 1;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    
    public void SetShowEnemyHealthBars(bool show)
    {
        showEnemyHealthBars = show;
        
        // get every enemy in the scene
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.SetHealthBarVisibility(showEnemyHealthBars);
        }
        
        PlayerPrefs.SetInt("showEnemyHealthBars", showEnemyHealthBars ? 1 : 0);
    }
    
    public void ShowDamageNumbers(bool show)
    {
        showDamageNumbers = show;
        PlayerPrefs.SetInt("showDamageNumbers", showDamageNumbers ? 1 : 0);
    }

    public void BindHyperScale()
    {
        var value = hyperModeToggle.isOn;
        if (!value)
        {
            GameTime.hyperModeTimeScale = 1f;
            isHyperMode = false;
            hyperText.SetActive(false);
        }
        else
        {
            GameTime.hyperModeTimeScale = 2f;
            isHyperMode = true;
            hyperText.SetActive(true);
        }
        PlayerPrefs.SetInt("isHyperMode", isHyperMode ? 1 : 0);
    }
}
