using UnityEngine;
using UnityEngine.UI;

public class EscButtonSelector : MonoBehaviour
{
    public Button backButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButton.onClick.Invoke();
        }
    }
}