using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(Stats))]
    public class Armor : MonoBehaviour
    {
        private Stat _armorStat;
        
        private void Start()
        {
            _armorStat = GetComponent<Stats>().GetStat(StatType.Armor);
        }

        public int CalculateArmorReduction(int damageAmount)
        {
            var armorMitigation = _armorStat.value / (_armorStat.value + 100f);

            damageAmount = Mathf.RoundToInt(damageAmount - armorMitigation);
            // We should never be invincible imo. hard cap to 1.
            if (damageAmount <= 0)
            {
                damageAmount = 1;
            }

            var newDamage = Mathf.Max(0, damageAmount - _armorStat.value);
            return newDamage;
        }
    }
}