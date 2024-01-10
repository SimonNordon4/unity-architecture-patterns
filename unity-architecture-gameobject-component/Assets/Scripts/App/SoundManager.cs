using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Definitions;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class SoundManager : MonoBehaviour
    {
        private Queue<AudioSource> _audioSources = new Queue<AudioSource>();



        private AudioSource GetAudioSource()
        {
            if (_audioSources.Count > 0)
            {
                return _audioSources.Dequeue();
            }
            else
            {
                AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
                return newAudioSource;
            }
        }

        private void ReturnAudioSource(AudioSource audioSource)
        {
            _audioSources.Enqueue(audioSource);
        }

        public void PlaySound(SoundDefinition soundDefinition)
        {
            var audioSource = GetAudioSource();
            // select a random clip
            audioSource.clip = soundDefinition.clips[Random.Range(0, soundDefinition.clips.Length)];
            audioSource.Play();
            StartCoroutine(ReturnAudioSourceWhenFinished(audioSource));
        }

        private IEnumerator ReturnAudioSourceWhenFinished(AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            ReturnAudioSource(audioSource);
        }
    }
}