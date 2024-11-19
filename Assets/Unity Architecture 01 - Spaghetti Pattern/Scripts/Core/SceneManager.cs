using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;

    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<SceneManager>();
            return _instance;
        }
    }
    
    public int mainMenuScene = 0;
    public int gameScene = 1;
    public int gameWon = 2;
    public int gameLost = 3;
    public int dungeonScene = 4;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadMainMenu()
    {
        LoadSceneAsync(mainMenuScene, LoadSceneMode.Single);
    }

    public void LoadGame() 
    {
        LoadSceneAsync(gameScene, LoadSceneMode.Single);
    }

    public void LoadGameWon()
    {
        LoadSceneAsync(gameWon, LoadSceneMode.Single);
    }

    public void LoadGameLost()
    {
        LoadSceneAsync(gameLost, LoadSceneMode.Single);
    }

    public void LoadEnvironment()
    {
        LoadSceneAsync(dungeonScene, LoadSceneMode.Additive);
    }
}