using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public GameObject graphy;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            var isActive = graphy.activeSelf;
            graphy.SetActive(!isActive);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Time.timeScale *= 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Time.timeScale *= 0.5f;
        }
    }
}
