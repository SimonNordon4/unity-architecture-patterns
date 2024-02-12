using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventString.
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerString")]
    public class EventListenerString : EventListenerGeneric<string>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<string>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<string>
        {
            [SerializeField] private ScriptableEventString _scriptableEvent = null;
            public override ScriptableEvent<string> ScriptableEvent => _scriptableEvent;

            [SerializeField] private StringUnityEvent _response = null;
            public override UnityEvent<string> Response => _response;
        }
        
        [System.Serializable]
        public class StringUnityEvent : UnityEvent<string>
        {
        }
    }
}