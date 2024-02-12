using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventBool
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerBool")]
    public class EventListenerBool : EventListenerGeneric<bool>
    {
        [FormerlySerializedAs("m_eventResponses")]
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<bool>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<bool>
        {
            [FormerlySerializedAs("mScriptableEvent")]
            [SerializeField] private ScriptableEventBool _scriptableEvent = null;
            public override ScriptableEvent<bool> ScriptableEvent => _scriptableEvent;

            [FormerlySerializedAs("m_response")]
            [SerializeField] private BoolUnityEvent _response = null;
            public override UnityEvent<bool> Response => _response;
        }
        
        [System.Serializable]
        public class BoolUnityEvent : UnityEvent<bool>
        {
            
        }
    }
}