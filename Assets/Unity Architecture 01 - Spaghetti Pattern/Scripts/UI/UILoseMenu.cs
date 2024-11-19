using TMPro;
using UnityArchitecture.SpaghettiPattern;
using UnityEngine;

public class UILoseMenu : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI timerText;
    [SerializeField]private TextMeshProUGUI enemiesKilledText;

    private void Start()
    {
        timerText.text = $"Time Alive: {GameManager.Instance.roundTime}";
        enemiesKilledText.text = $"Enemies Killed: {EnemyManager.Instance.totalEnemiesKilled}";
    }
}