using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventVector2
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerVector2")]
    public class EventListenerVector2 : EventListenerGeneric<Vector2>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<Vector2>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<Vector2>
        {
            [SerializeField] private ScriptableEventVector2 _scriptableEvent = null;
            public override ScriptableEvent<Vector2> ScriptableEvent => _scriptableEvent;

            [SerializeField] private Vector2UnityEvent _response = null;
            public override UnityEvent<Vector2> Response => _response;
        }
        
        [System.Serializable]
        public class Vector2UnityEvent : UnityEvent<Vector2>
        {
        }

    }
}