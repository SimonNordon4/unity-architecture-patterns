using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventGameObject
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerGameObject")]
    public class EventListenerGameObject : EventListenerGeneric<GameObject>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<GameObject>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<GameObject>
        {
            [SerializeField] private ScriptableEventGameObject _scriptableEvent = null;
            public override ScriptableEvent<GameObject> ScriptableEvent => _scriptableEvent;

            [SerializeField] private GameObjectUnityEvent _response = null;
            public override UnityEvent<GameObject> Response => _response;
        }
        
        [System.Serializable]
        public class GameObjectUnityEvent : UnityEvent<GameObject>
        {
        }
    }
}