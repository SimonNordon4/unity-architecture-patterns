using TMPro;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class UITotalEnemiesKilledText : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI text;

        private void OnEnable()
        {
            text.text = $"Enemies Killed: {EnemyDirector.Instance.TotalEnemiesKilled}";
        }
    }
}