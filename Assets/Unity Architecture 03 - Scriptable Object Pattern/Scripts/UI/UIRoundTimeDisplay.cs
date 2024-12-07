using TMPro;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UIRoundTimeDisplay : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI roundTimeText;

        private void OnEnable()
        {
            roundTimeText.text = $"Time Alive: {Mathf.FloorToInt(RoundTimer.Instance.CurrentTime / 60):00}:{Mathf.FloorToInt(RoundTimer.Instance.CurrentTime % 60):00}"; 
        }
    }
}