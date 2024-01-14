using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.App
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private RectTransform notificationContainer;
        [SerializeField] private TextMeshProUGUI notificationTextUGUI;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private float animationDuration = 0.5f;

        private readonly Queue<string> _notificationQueue = new Queue<string>();

        private void Start()
        {
            notificationContainer.gameObject.SetActive(false);
        }

        public void ScheduleNotification(string notificationText)
        {
            Debug.Log("ScheduleNotification");
            _notificationQueue.Enqueue(notificationText);
            if (!notificationContainer.gameObject.activeInHierarchy)
            {
                // enable
                notificationContainer.gameObject.SetActive(true);
                StartCoroutine(ShowNotification());
            }
        }

        private IEnumerator ShowNotification()
        {
            while (_notificationQueue.Count > 0)
            {
                notificationTextUGUI.text = _notificationQueue.Dequeue();
                yield return StartCoroutine(Move(startPosition, endPosition, animationDuration));
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(Move(endPosition, startPosition, animationDuration));
            }

            notificationContainer.gameObject.SetActive(false);
        }

        private IEnumerator Move(Vector2 start, Vector2 end, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                notificationContainer.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            notificationContainer.anchoredPosition = end;
        }
    }
}