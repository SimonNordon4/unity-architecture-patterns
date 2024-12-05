using TMPro;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    
    public class UIDamageNumber : MonoBehaviour
    {
        [SerializeField]private DamageReceiver damageReceiver;
        [SerializeField]private TextMeshProUGUI damageText;
        [SerializeField]private Color damageColor = Color.yellow;
        [SerializeField]private Color critColor = Color.red;

        private void OnEnable()
        {
            damageReceiver.OnDamageReceived += ProcessDamage;
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived -= ProcessDamage;
        }

        private void ProcessDamage(int damage, bool isCritical)
        {
            
        }
    }
}