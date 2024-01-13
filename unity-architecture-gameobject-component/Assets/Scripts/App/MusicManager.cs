using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.App
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private AudioClip[] musicTracks;
        
        [SerializeField] private float musicVolume = 0.5f;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = musicVolume;
        }

        private void OnEnable()
        {
            gameState.OnGameStart += PlayRandomTrack;
            gameState.OnGamePause += PauseTrack;
            gameState.OnGameResume += ResumeTrack;
            gameState.OnGameQuit += StopTrack;
            gameState.OnGameLost += StopTrack;
            gameState.OnGameWon += StopTrack;
        }

        private void OnDisable()
        {
            gameState.OnGameStart -= PlayRandomTrack;
            gameState.OnGamePause -= PauseTrack;
            gameState.OnGameResume -= ResumeTrack;
            gameState.OnGameQuit -= StopTrack;
            gameState.OnGameLost -= StopTrack;
            gameState.OnGameWon -= StopTrack;
        }
        
        private void ResumeTrack()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }

        private void PlayRandomTrack()
        {
            var randomIndex = Random.Range(0, musicTracks.Length);
            _audioSource.clip = musicTracks[randomIndex];
            _audioSource.loop = true;
            _audioSource.volume = musicVolume;
            _audioSource.Play();
        }

        private void PauseTrack()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }

        private void StopTrack()
        {
            _audioSource.Stop();
        }
        
        public void SetVolume(float volume)
        {
            Debug.Log($"Setting volume to {volume}");
            volume = Mathf.Clamp01(volume);
            musicVolume = volume;
            if(_audioSource != null)
                _audioSource.volume = volume;
        }
    }
}