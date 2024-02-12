using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventNoParam.
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerNoParam")]
    public class EventListenerNoParam : EventListenerBase
    {
        [SerializeField] private EventResponse[] _eventResponses = null;

        private readonly Dictionary<ScriptableEventNoParam, EventResponse> _dictionary =
            new Dictionary<ScriptableEventNoParam, EventResponse>();

        protected override void ToggleRegistration(bool toggle)
        {
            foreach (var eventResponse in _eventResponses)
            {
                if (toggle)
                {
                    eventResponse.ScriptableEvent.RegisterListener(this);
                    if (!_dictionary.ContainsKey(eventResponse.ScriptableEvent))
                        _dictionary.Add(eventResponse.ScriptableEvent, eventResponse);
                }
                else
                {
                    eventResponse.ScriptableEvent.UnregisterListener(this);
                    if (_dictionary.ContainsKey(eventResponse.ScriptableEvent))
                        _dictionary.Remove(eventResponse.ScriptableEvent);
                }
            }
        }

        internal void OnEventRaised(ScriptableEventNoParam eventRaised, bool debug = false)
        {
            var eventResponse = _dictionary[eventRaised];
             if (eventResponse.Delay > 0)
             {
                 if (gameObject.activeInHierarchy)
                     StartCoroutine(Cr_DelayInvokeResponse(eventRaised, eventResponse, debug));
                 else
                     DelayInvokeResponseAsync(eventRaised, eventResponse, debug, _cancellationTokenSource.Token);
             }
             else
                InvokeResponse(eventRaised, eventResponse, debug);
        }

        private IEnumerator Cr_DelayInvokeResponse(ScriptableEventNoParam eventRaised, EventResponse eventResponse,
            bool debug)
        {
            yield return new WaitForSeconds(eventResponse.Delay);
            InvokeResponse(eventRaised, eventResponse, debug);
        }

        private async void DelayInvokeResponseAsync(ScriptableEventNoParam eventRaised, EventResponse eventResponse,
            bool debug, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay((int)(eventResponse.Delay * 1000), cancellationToken);
                InvokeResponse(eventRaised, eventResponse, debug);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void InvokeResponse(ScriptableEventNoParam eventRaised, EventResponse eventResponse, bool debug)
        {
            eventResponse.Response?.Invoke();
            if (debug)
                Debug(eventRaised);
        }

        [System.Serializable]
        internal struct EventResponse
        {
            [Min(0)] [Tooltip("Delay in seconds before invoking the response.")]
            public float Delay;

            public ScriptableEventNoParam ScriptableEvent;
            public UnityEvent Response;
        }

        #region Debugging

        private void Debug(ScriptableEventNoParam eventRaised)
        {
            var response = _dictionary[eventRaised].Response;
            var registeredListenerCount = response.GetPersistentEventCount();

            for (var i = 0; i < registeredListenerCount; i++)
            {
                var debugText = "<color=#f75369>[Event] ";
                debugText += eventRaised.name;
                debugText += " => </color>";
                debugText += response.GetPersistentTarget(i);
                debugText += ".";
                debugText += response.GetPersistentMethodName(i);
                debugText += "()";
                UnityEngine.Debug.Log(debugText, gameObject);
            }
        }

        public override bool ContainsCallToMethod(string methodName)
        {
            var containsMethod = false;
            foreach (var eventResponse in _eventResponses)
            {
                var registeredListenerCount = eventResponse.Response.GetPersistentEventCount();

                for (int i = 0; i < registeredListenerCount; i++)
                {
                    if (eventResponse.Response.GetPersistentMethodName(i) == methodName)
                    {
                        var debugText = $"<color=#f75369>{methodName}()</color>";
                        debugText += " is called by the event: <color=#f75369>";
                        debugText += eventResponse.ScriptableEvent.name;
                        debugText += "</color>";
                        UnityEngine.Debug.Log(debugText, gameObject);
                        containsMethod = true;
                        break;
                    }
                }
            }

            return containsMethod;
        }

        #endregion
    }
}