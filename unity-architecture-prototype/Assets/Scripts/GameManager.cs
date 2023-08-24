using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [Header("Round")]
        public float roundDuration = 20f;
        private float _roundTime;

        public bool isPaused = false;
        
        public Vector2 levelBounds = new Vector2(25f, 25f);

        [Header("UI")] 
        public TextMeshProUGUI roundTimeText;


        private void Update()
        {
            _roundTime += Time.deltaTime;
            roundTimeText.text = $"Round Time: {(int)(roundDuration - _roundTime)}";
        }
    }
}