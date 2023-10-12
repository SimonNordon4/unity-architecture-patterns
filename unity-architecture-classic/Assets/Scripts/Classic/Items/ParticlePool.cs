using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace Classic.Items
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particlePrefab;
        private readonly Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

        public ParticleSystem Get(Vector3 position = new())
        {
            ParticleSystem particle;
            if (pool.Count > 0)
            {
                particle = pool.Dequeue();
            }
            else
            {
                particle = Instantiate(particlePrefab, transform);
            }

            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            particle.Play();

            // Start a coroutine to return the particle system to the pool after it's done playing
            StartCoroutine(ReturnAfterPlay(particle));

            return particle;
        }

        private IEnumerator ReturnAfterPlay(ParticleSystem particle)
        {
            // Wait for the duration of the particle system
            yield return new WaitForSeconds(particle.main.duration);
            Return(particle);
        }

        public void Return(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
            pool.Enqueue(particle);
        }
    }
}