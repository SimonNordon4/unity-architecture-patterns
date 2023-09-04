using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ReviveTextController : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private void OnEnable()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _textMeshProUGUI.text = "Revives: " +((int)GameManager.instance.revives.value).ToString();
    }
}