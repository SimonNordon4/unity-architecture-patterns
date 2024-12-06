using TMPro;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UIRoundTimer : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI roundTimerText;
        [SerializeField]private RoundTimer roundTimer;
        private void Update()
        {
            roundTimerText.text = $"{(int)(roundTimer.CurrentTime / 60):00}:{(int)(roundTimer.CurrentTime % 60):00}";
        }
    }
}
