using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityArchitecture.ScriptableObjectPattern
{
    /// <summary>
    /// Manages scene loading and unloading in the game, ensuring a singleton instance for persistent access.
    /// </summary>
    public class SceneLoader : ScriptableObject
    {
        public readonly int mainMenuScene = 0;
        public readonly int gameScene = 1;
        public readonly int gameWon = 2;
        public readonly int gameLost = 3;
        public readonly int dungeonScene = 4;


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
