using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Stats))]
    [RequireComponent(typeof(Movement))]
    public abstract class MoveBase : MonoBehaviour
    {
        protected Movement movement;
        protected Stat speedStat;

        protected void Awake()
        {
            movement = GetComponent<Movement>();
            speedStat = GetComponent<Stats>().GetStat(StatType.Speed);
        }
    }
}