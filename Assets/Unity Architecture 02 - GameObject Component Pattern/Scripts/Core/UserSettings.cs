using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UserSettings : MonoBehaviour
    {
        private static UserSettings _instance;

        public static UserSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UserSettings");
                    _instance = obj.AddComponent<UserSettings>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        [field:SerializeField] 
        public bool ShowDamageNumbers {get; private set;} = true;
        [field:SerializeField] 
        public bool ShowEnemyHealthBars {get; private set;} = true;
        [field:SerializeField] 
        public float MusicVolume {get; private set;} = 1f;
        [field:SerializeField] 
        public float SfxVolume {get; private set;} = 1f;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = volume;
            SaveSettings();
        }

        public void SetShowEnemyHealthBars(bool show)
        {
            ShowEnemyHealthBars = show;
            //TODO: add event
            SaveSettings();
        }

        public void SetShowDamageNumbers(bool show)
        {
            ShowDamageNumbers = show;
            SaveSettings();
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
            PlayerPrefs.SetInt("ShowEnemyHealthBars", ShowEnemyHealthBars ? 1 : 0);
            PlayerPrefs.SetInt("ShowDamageNumbers", ShowDamageNumbers ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
            ShowEnemyHealthBars = PlayerPrefs.GetInt("ShowEnemyHealthBars", 1) == 1;
            ShowDamageNumbers = PlayerPrefs.GetInt("ShowDamageNumbers", 1) == 1;
        }
    }
}