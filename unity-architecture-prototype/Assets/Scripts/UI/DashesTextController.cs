using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DashesTextController : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    
    private void OnEnable()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var dashes = ((int)GameManager.instance.dashes.value);
        var color = dashes > 0 ? new Color(0.66f,1f,0.66f): new Color(1f,0.5f,0.5f);
        var htmlColor = ColorUtility.ToHtmlStringRGB(color);

        _textMeshProUGUI.text = $"Dashes: <color=#{htmlColor}>{dashes}</color>";
    }
}
