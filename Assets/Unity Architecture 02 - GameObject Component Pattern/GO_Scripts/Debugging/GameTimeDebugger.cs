using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;

public class GameTimeDebugger : MonoBehaviour
{
    [SerializeField]
    private KeyCode speedUp = KeyCode.RightArrow;
    [SerializeField]
    private KeyCode slowDown = KeyCode.LeftArrow;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(speedUp))
        {
            Debug.Log("Slow down");
            GameTime.hyperModeTimeScale *= 0.5f;
        }
        
        if (Input.GetKeyDown(slowDown))
        {
            Debug.Log("Speed up");
            GameTime.hyperModeTimeScale *= 2f;
        }
    }
}
