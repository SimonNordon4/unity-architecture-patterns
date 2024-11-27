using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopInOnEnable : MonoBehaviour
{
    public Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = 0.01f * Vector3.one;
        StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        // quadrtatic pop in the scale back to one
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.localScale = Vector3.Lerp(0.01f * Vector3.one, targetScale, t * t);
            yield return new WaitForEndOfFrame();
        }
        
        transform.localScale = targetScale;
        yield return null;
    }

}
