using System.Collections.Generic;
using UnityEngine;
namespace UnityArchitecture.SpaghettiPattern
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioManager>();
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
        public int maxEnemyVolume = 20;
        private int musicIndex = 0;

        public AudioClip[] gameMusic;
        
        void Start()
        {
            _audioSources.Clear();
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.clip = gameMusic[Random.Range(0, gameMusic.Length)];
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
            if(_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
            }
            _musicSource.clip = gameMusic[musicIndex];
            _musicSource.volume = 0.1f * GameManager.Instance.musicVolume;
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
 
            
            if (GameManager.Instance.isGameActive)
            {
                _musicVolume = Mathf.Lerp(_musicVolume, GameManager.Instance.musicVolume, Time.deltaTime);
            }
            else
            {
                _musicVolume = Mathf.Lerp(_musicVolume, 0.1f * GameManager.Instance.musicVolume, 2f * Time.deltaTime);
            }
            
            _musicSource.volume = _musicVolume;
        }


        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                Debug.LogWarning("Audio clip is null", this);
                return;
            }
            var source = GetAudioSource();
            source.clip = audioClip;
            source.volume = GameManager.Instance.sfxVolume;
            source.pitch = Random.Range(0.8f, 1.2f); // Added random pitch shift
            source.PlayOneShot(source.clip);
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
            _buttonSource.volume = 0.5f * GameManager.Instance.sfxVolume;
            _buttonSource.PlayOneShot(buttonHover);
        }
        
        public void ButtonClick()
        {
            _buttonSource.volume = 1f * GameManager.Instance.sfxVolume;
            _buttonSource.PlayOneShot(buttonClick);
        }
    }
}