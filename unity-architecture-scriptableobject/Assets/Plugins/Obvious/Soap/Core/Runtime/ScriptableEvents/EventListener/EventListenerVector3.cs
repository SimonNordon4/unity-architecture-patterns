using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventVector3
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerVector3")]
    public class EventListenerVector3 : EventListenerGeneric<Vector3>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<Vector3>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<Vector3>
        {
            [SerializeField] private ScriptableEventVector3 _scriptableEvent = null;
            public override ScriptableEvent<Vector3> ScriptableEvent => _scriptableEvent;

            [SerializeField] private Vector3UnityEvent _response = null;
            public override UnityEvent<Vector3> Response => _response;
        }
        
        [System.Serializable]
        public class Vector3UnityEvent : UnityEvent<Vector3>
        {
        }
    }
}