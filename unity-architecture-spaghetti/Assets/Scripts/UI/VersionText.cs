


using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    public string version = "0.1.1";
    public TextMeshProUGUI versionText;

    private void Start()
    {
        versionText.text = version;
    }
}
