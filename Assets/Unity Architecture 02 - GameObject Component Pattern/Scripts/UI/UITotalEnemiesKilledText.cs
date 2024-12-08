using TMPro;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class UITotalEnemiesKilledText : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI text;
        [SerializeField]private EnemyDirector director;

        private void OnEnable()
        {
            text.text = $"Enemies Killed: {director.TotalEnemiesKilled}";
        }
    }
}