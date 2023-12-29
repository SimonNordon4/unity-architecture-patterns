using Classic.Game;
using Classic.Utility;
using TMPro;
using UnityEngine;

namespace Classic.UI
{
    public class UIGoldAmount : MonoBehaviour
    {
        [SerializeField] private Gold gold;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private string label = "Gold: ";

        private void OnEnable()
        {
            text.text = label + gold.amount.ToString();
            gold.OnGoldChanged+=(OnGoldChanged);
        }

        private void OnGoldChanged(int newGold)
        {
            text.text = label + newGold;
        }

        private void OnDisable()
        {
            gold.OnGoldChanged+=(OnGoldChanged);
        }
    }
}