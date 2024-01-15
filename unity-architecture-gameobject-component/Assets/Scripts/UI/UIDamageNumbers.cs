using System.Collections;
using System.Collections.Generic;
using GameplayComponents;
using GameplayComponents.Actor;
using GameplayComponents.Life;
using Pools;
using TMPro;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIDamageNumbers : GameplayComponent
    {
        [SerializeField] private ActorPool actorPool;
        [SerializeField] private UITextPool textPool;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private DamageHandler playerDamageHandler;

        [SerializeField] private Color dodgeColor;
        [SerializeField] private Color blockColor;
        [SerializeField] private Color damageColor;

        [SerializeField] private float numberTime = 0.8f;
        [SerializeField] private float numberSize = 1.2f;


        private void OnEnable()
        {
            actorPool.OnActorGet += OnActorGet;
            actorPool.OnActorReturn += OnActorReturn;
            playerDamageHandler.OnDamage += OnDamage;
            playerDamageHandler.OnBlock += OnBlock;
            playerDamageHandler.OnDodge += OnDodge;
        }

        private void OnActorGet(PoolableActor actor)
        {
            if (!actor.TryGetComponent<DamageHandler>(out var damageHandler)) return;
            damageHandler.OnDamage += OnDamage;
            damageHandler.OnBlock += OnBlock;
            damageHandler.OnDodge += OnDodge;
        }

        private void OnDodge(Vector3 worldPosition)
        {
            var text = textPool.GetText();
            text.text = "Dodge";
            text.color = dodgeColor;
            text.transform.position = GetScreenPosition(worldPosition);
            StartCoroutine(AnimateText(text,worldPosition, numberTime));
        }

        private void OnBlock(Vector3 worldPosition)
        {
            var text = textPool.GetText();
            text.transform.position = GetScreenPosition(worldPosition);
            text.text = "Block";
            text.color = blockColor;
            StartCoroutine(AnimateText(text,worldPosition, numberTime));
        }

        private void OnDamage(Vector3 worldPosition, int damage)
        {
            var text = textPool.GetText();
            text.transform.position = GetScreenPosition(worldPosition);
            text.text = damage.ToString();
            text.color = damageColor;
            StartCoroutine(AnimateText(text, worldPosition, numberTime));
        }

        private Vector3 GetScreenPosition(Vector3 worldPosition)
        {
            var screenPosition = gameCamera.WorldToScreenPoint(worldPosition);
            screenPosition.x += 30;
            return screenPosition;
        }

        private IEnumerator AnimateText(TextMeshProUGUI text, Vector3 worldPosition, float duration)
        {
            float elapsedTime = 0;
            var originalPosition = GetScreenPosition(worldPosition);
            var targetPosition = originalPosition + new Vector3(0, 100, 0); // Move 100 units upwards
            var originalScale = text.transform.localScale;
            var targetScale = originalScale * numberSize; // Double the size

            while (elapsedTime < duration)
            {
                // Calculate the current time ratio
                float timeRatio = elapsedTime / duration;

                // Recalculate the original position based on the world position
                originalPosition = GetScreenPosition(worldPosition);
                targetPosition = originalPosition + new Vector3(0, 100, 0); // Move 100 units upwards

                // Interpolate position and scale based on the time ratio
                text.transform.position = Vector3.Lerp(originalPosition, targetPosition, timeRatio);
                text.transform.localScale = Vector3.Lerp(originalScale, targetScale, timeRatio);

                // Wait for the next frame
                yield return null;
                // Increase elapsed time
                elapsedTime += Time.deltaTime;
            }

            // Ensure the final position and scale are correctly set
            text.transform.position = targetPosition;
            text.transform.localScale = targetScale;

            // Return the text to the pool
            textPool.ReturnDamageNumber(text);
        }

        private void OnActorReturn(PoolableActor actor)
        {
            if (!actor.TryGetComponent<DamageHandler>(out var damageHandler)) return;
            damageHandler.OnDamage -= OnDamage;
            damageHandler.OnBlock -= OnBlock;
            damageHandler.OnDodge -= OnDodge;
        }

        private void OnDisable()
        {
            actorPool.OnActorGet -= OnActorGet;
            actorPool.OnActorReturn -= OnActorReturn;
            playerDamageHandler.OnDamage -= OnDamage;
            playerDamageHandler.OnBlock -= OnBlock;
            playerDamageHandler.OnDodge -= OnDodge;
        }
    }
}