using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    
    public class UIDamageNumber : MonoBehaviour
    {
        [SerializeField]private DamageReceiver damageReceiver;
        [SerializeField]private TextMeshProUGUI damageText;
        [SerializeField]private Color damageColor = Color.yellow;
        [SerializeField]private Color critColor = Color.red;
        private Vector3 _originalPosition;

        private void OnEnable()
        {
            damageReceiver.OnDamageReceived += ProcessDamage;
        }

        private void Start()
        {
            _originalPosition = transform.localPosition;
        }

        private void OnDisable()
        {
            damageReceiver.OnDamageReceived -= ProcessDamage;
            
            StopAllCoroutines();
            // On Disable we want to reset the position of the text and disable it
            damageText.transform.position = _originalPosition;
            damageText.text = "";
            damageText.gameObject.SetActive(false);
        }

        private void ProcessDamage(int damage, bool isCritical)
        {
            StartCoroutine(ShowDamageText(damage,isCritical));
        }
        
        private IEnumerator ShowDamageText(int damage, bool isCritical = false)
        {
            damageText.color = isCritical ? critColor : damageColor;
            damageText.text = damage.ToString();
            GameObject o = damageText.gameObject;
            o.SetActive(true);
            var elapsedTime = 0f;
            var t = o.transform;

            var startPosition = Vector3.right;
            var targetPosition = Vector3.up + Vector3.right * 1f;

            var startScale = Vector3.one * 0.25f;
            var targetScale = Vector3.one;

            while (elapsedTime < 0.6f)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = elapsedTime / 0.4f;
                var inversedQuadraticTime = 1 - Mathf.Pow(1 - normalizedTime, 2);
                t.position = Vector3.Lerp(startPosition + transform.position, targetPosition + transform.position, inversedQuadraticTime);
                t.localScale = Vector3.Lerp(startScale, targetScale, inversedQuadraticTime);
                yield return new WaitForEndOfFrame();
            }
            damageText.gameObject.SetActive(false);
            yield return null;
        }

    }
}