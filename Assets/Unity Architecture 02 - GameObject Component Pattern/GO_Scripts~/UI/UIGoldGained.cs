using GameObjectComponent.Game;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIGoldGained : MonoBehaviour
    {
        [SerializeField] private RoundEndGoldHandler handler;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private string label = "+";

        private void OnEnable()
        {
            text.text = label + handler.lastRoundGoldAdded.ToString();
        }
    }
}