using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class PopUpScheduler : MonoBehaviour
    {
        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private TextMeshProUGUI popupTextUGUI;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private float animationDuration = 0.5f;

        private readonly Queue<string> _notificationQueue = new Queue<string>();

        private void Start()
        {
            popupContainer.gameObject.SetActive(false);
        }

        public void SchedulePopup(string notificationText)
        {
            Debug.Log("ScheduleNotification");
            _notificationQueue.Enqueue(notificationText);
            if (!popupContainer.gameObject.activeInHierarchy)
            {
                // enable
                popupContainer.gameObject.SetActive(true);
                StartCoroutine(ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            while (_notificationQueue.Count > 0)
            {
                popupTextUGUI.text = _notificationQueue.Dequeue();
                yield return StartCoroutine(Move(startPosition, endPosition, animationDuration));
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(Move(endPosition, startPosition, animationDuration));
            }

            popupContainer.gameObject.SetActive(false);
        }

        private IEnumerator Move(Vector2 start, Vector2 end, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                popupContainer.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            popupContainer.anchoredPosition = end;
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Test Popup")]
        public void TestPopup()
        {
            SchedulePopup("Test Notification");
        }
        #endif
    }
}