using System.Collections;
using UnityEngine;

public class ActorViewHandler : MonoBehaviour
{
    [SerializeField] private GameObject enemyMesh;
    [SerializeField] private ParticleSystem spawnInParticle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private TrailRenderer trailRenderer;

    public void OnEnable()
    {
        StartCoroutine(SpawnIn());
    }

    private IEnumerator SpawnIn()
    {
        enemyMesh.SetActive(false);
        spawnInParticle.Play();
        trailRenderer.Clear();
        yield return new WaitForSeconds(1f);
        deathParticle.Play();
    }
}