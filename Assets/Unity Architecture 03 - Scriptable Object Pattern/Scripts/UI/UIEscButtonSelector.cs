using UnityEngine;
using UnityEngine.UI;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIEscButtonSelector : MonoBehaviour
    {
        public Button backButton;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) backButton.onClick.Invoke();
        }
    }
}