using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            FindObjectOfType<PlayerController>().TakeDamage(1000);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
           // double time speed
           Time.timeScale *= 2;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // half time speed
            Time.timeScale /= 2;
        }
    }
}
