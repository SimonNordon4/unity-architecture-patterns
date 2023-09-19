using System.Collections.Generic;
using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }

                return _instance;
            }
        }
        
        private AudioSource _musicSource;
        private List<AudioSource> _audioSources = new();

        private AudioSource _buttonSource;
        public AudioClip buttonHover;
        public AudioClip buttonClick;
        
        private float _musicVolume = 0f;
        private float _actionVolume = 0f;
        private EnemyManager _enemyManager;
        public int maxEnemyVolume = 20;
        private int musicIndex = 0;
        
        public AudioClip[] gameMusic;
        
        void Start()
        {
            _audioSources.Clear();
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.clip = gameMusic[Random.Range(0, gameMusic.Length)];
            _enemyManager = FindObjectOfType<EnemyManager>();
            _buttonSource = gameObject.AddComponent<AudioSource>();

            _audioSources = new List<AudioSource>();
            for (int i = 0; i < 5; i++)
            {
                _audioSources.Add(gameObject.AddComponent<AudioSource>());
            }
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void OnStartGame()
        {
            // pick a random music clip and play it
            musicIndex = Random.Range(0, gameMusic.Length);
            _musicSource.clip = gameMusic[musicIndex];
            _musicSource.volume = 0.1f * SettingsManager.instance.musicVolume;
            _musicSource.Play();
        }

        private void Update()
        {
            // check how far the music clip is to see if it's near the end
            // if it is, pick a new clip and play it
            if (_musicSource.clip.length - _musicSource.time < 0.1f)
            {
                musicIndex = (musicIndex + 1) % gameMusic.Length;
                _musicSource.clip = gameMusic[musicIndex];
                _musicSource.Play();
            }
 
            
            if (_enemyManager.enemies.Count > 8)
            {
                _actionVolume = (_enemyManager.enemies.Count / maxEnemyVolume) * 0.5f;
            }
            
            if (GameManager.instance.isGameActive)
            {
                _musicVolume = Mathf.Lerp(_musicVolume, (0.3f + _actionVolume) * SettingsManager.instance.musicVolume, Time.deltaTime);
            }
            else
            {
                _musicVolume = Mathf.Lerp(_musicVolume, 0.1f * SettingsManager.instance.musicVolume, 2f * Time.deltaTime);
            }
            
            _musicSource.volume = _musicVolume;
        }


        public void PlaySound(SoundDefinition definition)
        {
            var source = GetAudioSource();
            source.clip = definition.clips[Random.Range(0, definition.clips.Length)];
            source.volume = definition.volume * SettingsManager.instance.sfxVolume;
            source.pitch = Random.Range(definition.pitchVariation.x, definition.pitchVariation.y);
            source.Play();
        }

        private AudioSource GetAudioSource()
        {
            foreach (var source in _audioSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            
            var newSource = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(newSource);
            return newSource;
        }

        public void ButtonHover()
        {
            if(_buttonSource.isPlaying) return;
            _buttonSource.volume = 0.5f * SettingsManager.instance.sfxVolume;
            _buttonSource.PlayOneShot(buttonHover);
        }
        
        public void ButtonClick()
        {
            _buttonSource.volume = 1f * SettingsManager.instance.sfxVolume;
            _buttonSource.PlayOneShot(buttonClick);
        }
    }
