using System.Collections;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    public GameState gameState;
    public float lifeTime = 5f;
    private float _aliveTime = 0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (gameState.currentState != GameStateEnum.Active) return;
        
        _aliveTime += Time.deltaTime;
        if(_aliveTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
