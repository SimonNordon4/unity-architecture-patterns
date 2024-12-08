using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIRoundTimeDisplay : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI roundTimeText;
        [SerializeField]private RoundTimer roundTimer;

        private void OnEnable()
        {
            roundTimeText.text = $"Time Alive: {Mathf.FloorToInt(roundTimer.CurrentTime / 60):00}:{Mathf.FloorToInt(roundTimer.CurrentTime % 60):00}"; 
        }
    }
}