﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]private SoundEvent soundEvent;
        [SerializeField]private UserSettings userSettings;
        private Queue<AudioSource> _audioSources = new Queue<AudioSource>();

        private void OnEnable()
        {
            soundEvent.OnPlaySound.AddListener(PlaySound);
        }

        private void OnDisable()
        {
            soundEvent.OnPlaySound.RemoveListener(PlaySound);
        }

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

        public void PlaySound(AudioClip clip)
        {
            var source = GetAudioSource();
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null", this);
                return;
            }
            source.clip = clip;
            source.volume = userSettings.SfxVolume;
            source.pitch = Random.Range(0.8f, 1.2f); // Added random pitch shift
            source.PlayOneShot(source.clip);
            StartCoroutine(ReturnAudioSourceWhenFinished(source));
        }
        

        private IEnumerator ReturnAudioSourceWhenFinished(AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            ReturnAudioSource(audioSource);
        }
    }
}