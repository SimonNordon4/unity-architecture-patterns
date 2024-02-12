using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Game;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameTimeDebugger : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame)
        {
            Debug.Log("Slow down");
            GameTime.hyperModeTimeScale *= 0.5f;
        }
        
        if (Keyboard.current[Key.RightArrow].wasPressedThisFrame)
        {
            Debug.Log("Speed up");
            GameTime.hyperModeTimeScale *= 2f;
        }
    }
}
