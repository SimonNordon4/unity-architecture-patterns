using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern  
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField]private UserSettings userSettings;
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
            gameState.onGameStarted.AddListener(PlayRandomTrack);
            gameState.onGamePaused.AddListener(PauseTrack);
            gameState.onGameResumed.AddListener(ResumeTrack);
            gameState.onGameLost.AddListener(StopTrack);
            gameState.onGameWon.AddListener(StopTrack);
        }

        private void OnDisable()
        {
            gameState.onGameStarted.RemoveListener(PlayRandomTrack);
            gameState.onGamePaused.RemoveListener(PauseTrack);
            gameState.onGameResumed.RemoveListener(ResumeTrack);
            gameState.onGameLost.RemoveListener(StopTrack);
            gameState.onGameWon.RemoveListener(StopTrack);
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
            _audioSource.volume = userSettings.MusicVolume * musicVolume;
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
    }
}