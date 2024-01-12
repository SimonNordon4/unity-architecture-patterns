using System;
using GameObjectComponent.Game;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectComponent.App
{
    public class Settings : PersistentComponent
    {
        [field:SerializeField] public bool showEnemyHealthBars {get;private set;} = true;
        [field:SerializeField] public bool showDamageNumbers {get;private set;} = true;
        [field:SerializeField] public bool hyperMode {get;private set;} = false;
        [field:SerializeField] public float musicVolume {get;private set;} = 1f;
        [field:SerializeField] public float sfxVolume {get;private set;} = 1f;

        public UnityEvent<bool> onShowEnemyHealthBarsChanged = new();
        public UnityEvent<bool> onShowDamageNumbersChanged = new();
        public UnityEvent<bool> onHyperModeChanged = new();
        public UnityEvent<float> onMusicVolumeChanged = new();
        public UnityEvent<float> onSfxVolumeChanged = new();
        
        public void SetShowEnemyHealthBars(bool show)
        {
            showEnemyHealthBars = show;
            onShowEnemyHealthBarsChanged?.Invoke(show);
        }
        
        public void SetShowDamageNumbers(bool show)
        {
            showDamageNumbers = show;
            onShowDamageNumbersChanged?.Invoke(show);
        }
        
        public void SetMusicVolume(float newVolume)
        {
            onHyperModeChanged?.Invoke(hyperMode);
            musicVolume = newVolume;
        }
        
        public void SetSfxVolume(float newVolume)
        {
            sfxVolume = newVolume;
        }
        
        public void SetHyperMode(bool hyper)
        {
            hyperMode = hyper;
            GameTime.hyperModeTimeScale = hyper ? 2f : 1f;
            onHyperModeChanged?.Invoke(hyper);
        }
            
        public override void Save()
        {
            // save
            var save = new SettingSave(this);
            var json = JsonUtility.ToJson(save);
            PlayerPrefs.SetString("settings", json);
        }

        public override void Load()
        {
            // load
            var json = PlayerPrefs.GetString($"settings_{id}", "");
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            
            var save = JsonUtility.FromJson<SettingSave>(json);
            
            SetShowEnemyHealthBars(save.showEnemyHealthBars);
            SetShowDamageNumbers(save.showDamageNumbers);
            SetHyperMode(save.hyperMode);
            SetMusicVolume(save.musicVolume);
            SetSfxVolume(save.sfxVolume);
        }
    
        [Serializable]
        public class SettingSave
        {
            public bool showEnemyHealthBars = true;
            public bool showDamageNumbers = true;
            
            public bool hyperMode = false;
            
            public float musicVolume = 1f;
            public float sfxVolume = 1f;
            
            public SettingSave(Settings settings)
            {
                showEnemyHealthBars = settings.showEnemyHealthBars;
                showDamageNumbers = settings.showDamageNumbers;
                hyperMode = settings.hyperMode;
                musicVolume = settings.musicVolume;
                sfxVolume = settings.sfxVolume;
            }
        }
    }
}