using GameplayComponents.Life;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameObjectComponent.Debugging
{
    public class LifeDebugger : DebugComponent
    {
        [field:SerializeField]public Health health { get; private set; }
        [field:SerializeField]public DamageHandler damageHandler { get; private set; }
        [field:SerializeField]public DeathHandler deathHandler { get; private set; }
        
        private void OnEnable()
        {
            health.OnHealthChanged += x => Print("Health changed to " + health.currentHealth);
            health.OnHealthDepleted += () => Print("Health depleted");
            deathHandler.onRevived.AddListener(() => Print("Revived"));
            deathHandler.onDeath.AddListener(() => Print("Died"));
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(LifeDebugger))]
    public class LifeDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var lifeDebugger = (LifeDebugger) target;
            if (GUILayout.Button("Damage"))
            {
                lifeDebugger.health.TakeDamage(1);
            }
            if (GUILayout.Button("Kill"))
            {
                lifeDebugger.health.TakeDamage(999);
            }
        }
    }
#endif
}