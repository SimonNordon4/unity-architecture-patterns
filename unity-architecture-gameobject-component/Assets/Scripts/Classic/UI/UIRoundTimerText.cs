using System.Collections;
using System.Collections.Generic;
using Classic.Game;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIRoundTimerText : MonoBehaviour
{
    [SerializeField]private RoundTimer roundTimer;
    private TextMeshProUGUI _text;
    
    private int _minutes;
    private int _seconds;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _minutes = Mathf.FloorToInt(roundTimer.roundTime / 60f);
        _seconds = Mathf.FloorToInt(roundTimer.roundTime % 60f);
        _text.text = $"Round Time: {_minutes:00}:{_seconds:00}";
    }

    private void LateUpdate()
    {
        // only update on the second
        if (Mathf.FloorToInt(roundTimer.roundTime) == Mathf.FloorToInt(roundTimer.roundTime - GameTime.deltaTime)) return;
        
        _minutes = Mathf.FloorToInt(roundTimer.roundTime / 60f);
        _seconds = Mathf.FloorToInt(roundTimer.roundTime % 60f);
        _text.text = $"Round Time: {_minutes:00}:{_seconds:00}";
    }

    private void OnValidate()
    {
        if (roundTimer == null)
        {
            roundTimer = FindObjectsByType<RoundTimer>(FindObjectsSortMode.None)[0];
        }
    }
}
