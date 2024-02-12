using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventColor
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerColor")]
    public class EventListenerColor : EventListenerGeneric<Color>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<Color>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<Color>
        {
            [SerializeField] private ScriptableEventColor _scriptableEvent = null;
            public override ScriptableEvent<Color> ScriptableEvent => _scriptableEvent;

            [SerializeField] private ColorUnityEvent _response = null;
            public override UnityEvent<Color> Response => _response;
        }
        
        [System.Serializable]
        public class ColorUnityEvent : UnityEvent<Color>
        {
        }
    }
}