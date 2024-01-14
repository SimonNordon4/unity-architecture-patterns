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
            Debug.Log($"SetShowEnemyHealthBars {show}");
            showEnemyHealthBars = show;
            onShowEnemyHealthBarsChanged?.Invoke(show);
        }
        
        public void SetShowDamageNumbers(bool show)
        {
            Debug.Log($"SetShowDamageNumbers {show}");
            showDamageNumbers = show;
            onShowDamageNumbersChanged?.Invoke(show);
        }
        
        public void SetMusicVolume(float newVolume)
        {
            Debug.Log($"SetMusicVolume {newVolume}");
            onMusicVolumeChanged?.Invoke(newVolume);
            musicVolume = newVolume;
        }
        
        public void SetSfxVolume(float newVolume)
        {
            Debug.Log($"SetSfxVolume {newVolume}");
            onSfxVolumeChanged?.Invoke(newVolume);
            sfxVolume = newVolume;
        }
        
        public void SetHyperMode(bool hyper)
        {
            Debug.Log($"SetHyperMode {hyper}");
            hyperMode = hyper;
            GameTime.hyperModeTimeScale = hyper ? 2f : 1f;
            onHyperModeChanged?.Invoke(hyper);
        }
            
        public override void Save()
        {
            // save
            var save = new SettingSave(this);
            var json = JsonUtility.ToJson(save);
            Debug.Log($"Saved settings {json}");
            PlayerPrefs.SetString($"settings_{id}", json);
        }

        public override void Load()
        {
            var json = PlayerPrefs.GetString($"settings_{id}", "");
            Debug.Log($"Loaded settings {json}");
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
        
        public override void Reset()
        {
            return;
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