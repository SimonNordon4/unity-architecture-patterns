using System.Threading;
using UnityEngine;

namespace Obvious.Soap
{
    /// <summary>
    /// Base class for all event listeners
    /// </summary>
    public abstract class EventListenerBase : MonoBehaviour
    {
        protected enum Binding
        {
            UNTIL_DESTROY,
            UNTIL_DISABLE
        }

        [SerializeField] protected Binding _binding = Binding.UNTIL_DESTROY;
        [SerializeField] protected bool _disableAfterSubscribing = false;
        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected abstract void ToggleRegistration(bool toggle);

        /// <summary>
        /// Returns true if the event listener contains a call to the method with the given name
        /// </summary>
        public abstract bool ContainsCallToMethod(string methodName);

        private void Awake()
        {
            if (_binding == Binding.UNTIL_DESTROY)
                ToggleRegistration(true);

            gameObject.SetActive(!_disableAfterSubscribing);
        }

        private void OnEnable()
        {
            if (_binding == Binding.UNTIL_DISABLE)
                ToggleRegistration(true);
        }

        private void OnDisable()
        {
            if (_binding == Binding.UNTIL_DISABLE)
            {
                ToggleRegistration(false);
                _cancellationTokenSource.Cancel();
            }
        }

        private void OnDestroy()
        {
            if (_binding == Binding.UNTIL_DESTROY)
            {
                ToggleRegistration(false);
                _cancellationTokenSource.Cancel();
            }
        }
    }
}