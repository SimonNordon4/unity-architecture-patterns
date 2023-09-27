using Classic.Core;
using UnityEngine;

namespace Classic.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameState state;
        
        private void OnEnable()
        {
            state.onGameStart.AddListener(() => mainMenu.SetActive(false));
            state.onGameReturnToMainMenu.AddListener(() => mainMenu.SetActive(true));
        }

        private void Start()
        {
            mainMenu.SetActive(true);
        }
    }
}