using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ActorDiedEvent : ScriptableEvent
    {
        [FormerlySerializedAs("OnEnemyDied")] public UnityEvent<GameObject> OnActorDied = new();

        public void Invoke(GameObject enemy)
        {
            OnActorDied?.Invoke(enemy);
        }
    }
}