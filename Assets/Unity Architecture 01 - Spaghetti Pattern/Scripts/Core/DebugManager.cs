using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
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

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                FindFirstObjectByType<PlayerManager>().TakeDamage(1000);
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

                // var gm = GameManager.instance;

                // gm.pistolDamage.initialValue = 999;
                // gm.pistolDamage.value = 999;

                // gm.pistolFireRate.initialValue = 3;
                // gm.pistolFireRate.value = 3;

                // gm.pistolRange.initialValue = 25;
                // gm.pistolRange.value = 25;

                // gm.block.value = 99999;
                // gm.block.initialValue = 99999;
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                // var gm = GameManager.instance;
                // gm.block.value = 99999;
                // gm.block.initialValue = 99999;
                // gm.pistolDamage.initialValue = 0;
                // gm.pistolDamage.value = 0;
                // gm.pistolKnockBack.initialValue = 0;
                // gm.pistolKnockBack.value = 0;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                // increase all stats by 10%
                // var gameManager = FindFirstObjectByType<GameManager>();
                // gameManager.playerMaxHealth.value *= 1.1f;
                // gameManager.playerSpeed.value *= 1.1f;
                // gameManager.pistolDamage.value *= 1.1f;
                // gameManager.pistolFireRate.value *= 1.1f;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                // increase all stats by 10%
                // var gameManager = FindFirstObjectByType<GameManager>();
                // gameManager.playerMaxHealth.value /= 1.1f;
                // gameManager.playerSpeed.value /= 1.1f;
                // gameManager.pistolDamage.value /= 1.1f;
                // gameManager.pistolFireRate.value /= 1.1f;
            }
#endif
        }
    }
}