using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Items;
using UnityEngine;


public class SpawnIndicatorController : MonoBehaviour
{
    public GameObject spawnIndicatorFinishedParticle;

    private void OnDestroy()
    {
            var p = Instantiate(spawnIndicatorFinishedParticle, transform.position, Quaternion.identity);
            p.transform.localScale = Vector3.one * 0.4f;
            AudioManager.instance.StartCoroutine(DestroyAfter(p));
    }

    private IEnumerator DestroyAfter(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
