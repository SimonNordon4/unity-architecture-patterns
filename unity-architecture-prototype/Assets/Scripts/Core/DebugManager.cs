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

        if (Input.GetKeyDown(KeyCode.U))
        {

            var gm = GameManager.instance;

            gm.pistolDamage.initialValue = 999;
            gm.pistolDamage.value = 999;

            gm.pistolFireRate.initialValue = 3;
            gm.pistolFireRate.value = 3;

            gm.pistolRange.initialValue = 25;
            gm.pistolRange.value = 25;
            
            gm.block.value = 99999;
            gm.block.initialValue = 99999;

        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // increase all stats by 10%
            var gameManager = FindObjectOfType<GameManager>();
            gameManager.playerMaxHealth.value *= 1.1f;
            gameManager.playerSpeed.value *= 1.1f;
            gameManager.pistolDamage.value *= 1.1f;
            gameManager.pistolFireRate.value *= 1.1f;
            gameManager.swordDamage.value *= 1.1f;
            gameManager.swordAttackSpeed.value *= 1.1f;
            gameManager.swordRange.value *= 1.1f;
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // increase all stats by 10%
            var gameManager = FindObjectOfType<GameManager>();
            gameManager.playerMaxHealth.value /= 1.1f;
            gameManager.playerSpeed.value/= 1.1f;
            gameManager.pistolDamage.value /= 1.1f;
            gameManager.pistolFireRate.value /= 1.1f;
            gameManager.swordDamage.value /= 1.1f;
            gameManager.swordAttackSpeed.value /= 1.1f;
            gameManager.swordRange.value /= 1.1f;
        }
    }
}
