using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(CombatTarget))]
    public class AutoTargeting : MonoBehaviour
    {
        private CombatTarget _target;
        private Stat _rangedStat;
        
        [SerializeField]private float updateInterval = 0.2f;
        private float _timeSinceLastUpdate = 0f;

        private void Start()
        {
            _target = GetComponent<CombatTarget>();
            _rangedStat = GetComponent<Stats>().GetStat(StatType.Range);
        }
        
        private void Update()
        {
            if(_timeSinceLastUpdate < updateInterval)
            {
                _timeSinceLastUpdate += Time.deltaTime;
                return;
            }
            
            _timeSinceLastUpdate = 0f;
            _target.GetClosestTarget(_rangedStat.value);
        }
        
    }
}