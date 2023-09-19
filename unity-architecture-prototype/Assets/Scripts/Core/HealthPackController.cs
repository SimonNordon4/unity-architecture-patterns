using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    public float lifeTime = 5f;

    private float _aliveTime = 0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameActive) return;
        
        _aliveTime += Time.deltaTime;
        if(_aliveTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
