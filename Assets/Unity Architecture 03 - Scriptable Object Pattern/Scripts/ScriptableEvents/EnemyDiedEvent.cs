using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class EnemyDiedEvent : ScriptableObject
    {
        public UnityEvent<GameObject> OnEnemyDied = new();

        public void Invoke(GameObject enemy)
        {
            OnEnemyDied?.Invoke(enemy);
        }
    }
}