using UnityEngine;

namespace Obvious.Soap
{
    [System.Serializable]
    public abstract class ScriptableEventBase : ScriptableBase
    {
        [Tooltip("Enable console logs when this event is raised.")]
        [SerializeField] 
        protected bool _debugLogEnabled = false;
        public bool DebugLogEnabled => _debugLogEnabled;
        
        public virtual System.Type GetGenericType { get; }
    }
}