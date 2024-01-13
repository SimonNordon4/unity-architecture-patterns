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
        
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = musicVolume;
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
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private void PlayRandomTrack()
        {
            var randomIndex = Random.Range(0, musicTracks.Length);
            audioSource.clip = musicTracks[randomIndex];
            audioSource.Play();
        }

        private void PauseTrack()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

        private void StopTrack()
        {
            audioSource.Stop();
        }
        
        public void SetVolume(float volume)
        {
            Debug.Log($"Setting volume to {volume}");
            volume = Mathf.Clamp01(volume);
            musicVolume = volume;
            audioSource.volume = volume;
        }
    }
}