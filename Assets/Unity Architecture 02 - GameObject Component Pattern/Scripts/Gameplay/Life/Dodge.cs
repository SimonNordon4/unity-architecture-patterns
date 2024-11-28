using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [RequireComponent(typeof(Stats))]
    public class Dodge : MonoBehaviour
    {
        private Stat _dodgeStat;
        public UnityEvent onDodged = new();
        
        private void Awake()
        {
            _dodgeStat = GetComponent<Stats>().GetStat(StatType.Dodge);
        }

        public bool CalculateDodge()
        {
            var chance = _dodgeStat.value;
            var random = Random.Range(0, 100);
            
            if (random > chance) return false;
            onDodged.Invoke();
            return true;
        }
    }
}