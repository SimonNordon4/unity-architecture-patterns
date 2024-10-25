using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    private int mainMenuScene = 0;
    private int gameScene = 1;
    private int dungeonScene = 2;

    public void Start()
    {
        if (GetActiveScene().buildIndex == gameScene)
        {
            LoadSceneAsync(dungeonScene, LoadSceneMode.Additive);       
        }
    }

    public void LoadMainMenu()
    {
        LoadSceneAsync(mainMenuScene, LoadSceneMode.Single);
    }

    public void LoadGame() 
    {
        LoadSceneAsync(gameScene, LoadSceneMode.Single);
    }
}