using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityArchitecture.GameObjectComponentPattern
{
    /// <summary>
    /// Manages scene loading and unloading in the game, ensuring a singleton instance for persistent access.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<SceneLoader>();
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(SceneLoader).Name);
                        _instance = singleton.AddComponent<SceneLoader>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }

        public int CurrentMainScene => SceneManager.GetActiveScene().buildIndex;
        public bool IsEnvironmentLoaded => SceneManager.GetSceneByBuildIndex(dungeonScene).isLoaded;

        public readonly int mainMenuScene = 0;
        public readonly int gameScene = 1;
        public readonly int gameWon = 2;
        public readonly int gameLost = 3;
        public readonly int dungeonScene = 4;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadMainMenu()
        {
            UnloadEnvironment();
            SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
        }

        public void LoadGame()
        {
            SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
        }

        public void LoadGameWon()
        {
            UnloadEnvironment();
            SceneManager.LoadScene(gameWon, LoadSceneMode.Single);
        }

        public void LoadGameLost()
        {
            UnloadEnvironment();
            SceneManager.LoadScene(gameLost, LoadSceneMode.Single);
        }

        public void LoadEnvironment()
        {
            if (!SceneManager.GetSceneByBuildIndex(dungeonScene).isLoaded)
            {
                Debug.Log("Loading Environment");
                SceneManager.LoadScene(dungeonScene, LoadSceneMode.Additive);
            }
        }

        private void UnloadEnvironment()
        {
            if (SceneManager.GetSceneByBuildIndex(dungeonScene).isLoaded)
            {
                SceneManager.UnloadSceneAsync(dungeonScene);
            }
        }
    }
}
