using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectComponent.UI
{
    public class UIRoundProgress : MonoBehaviour
    {
        [SerializeField]private RoundSpawner roundSpawner;
        [SerializeField]private WaveSpawner waveSpawner;
        
        [SerializeField]private TextMeshProUGUI waveText;
        [SerializeField]private Image waveMarkerPrefab;
        [SerializeField]private RectTransform waveMarkerContainer;
        
        private List<Image> _waveMarkers = new List<Image>();

        
        [SerializeField] private Image progressBar;
        [SerializeField]private RectTransform progressMarker;
        [SerializeField] private TextMeshProUGUI progressMarkerText;
        
        [SerializeField]private Color completedWaveColor;
        [SerializeField]private Color incompleteWaveColor;
        
        private float waveHeight => waveMarkerContainer.rect.height / (float)roundSpawner.totalWaves;
        private float actorHeight => waveHeight / (float)waveSpawner.totalActorsInWave;

        private void OnEnable()
        {
            waveSpawner.OnWaveCompleted += OnWaveCompleted;
            waveSpawner.onWaveActorDied.AddListener(OnWaveActorDied);
            CreateWaveMarkers();
            UpdateProgressBar();
            waveText.text = $"Wave: {roundSpawner.wavesCompleted + 1}/{roundSpawner.totalWaves}";
        }

        private void OnDisable()
        {
            waveSpawner.OnWaveCompleted -= OnWaveCompleted;
            waveSpawner.onWaveActorDied.RemoveListener(OnWaveActorDied);
        }
        
        private void CreateWaveMarkers()
        {
            var totalWaves = roundSpawner.totalWaves;
            var waveMarkerHeight = waveMarkerContainer.rect.height / (float)totalWaves;
            for (var i = 0; i < totalWaves; i++)
            {
                var waveMarker = Instantiate(waveMarkerPrefab, waveMarkerContainer);
                _waveMarkers.Add(waveMarker);
                var rectTransform = waveMarker.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, waveMarkerHeight * i + 1);
            }
            UpdateWaveMarkers();
        }

        private void OnWaveCompleted(Vector3 obj)
        {
            UpdateWaveMarkers();
        }

        private void OnWaveActorDied(Vector3 arg0)
        {
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            var progress = waveHeight * roundSpawner.wavesCompleted + actorHeight * waveSpawner.actorsKilledThisWave;
            progressMarker.anchoredPosition = new Vector2(0, progress);
            progressBar.fillAmount = progress / waveMarkerContainer.rect.height;
            progressMarkerText.text = (roundSpawner.wavesCompleted + 1).ToString();
        }
        
        private void UpdateWaveMarkers()
        {
            for (var i = 0; i < _waveMarkers.Count; i++)
            {
                _waveMarkers[i].color = i < roundSpawner.wavesCompleted ? completedWaveColor : incompleteWaveColor;
            }
            progressBar.fillAmount = (float)roundSpawner.wavesCompleted / roundSpawner.totalWaves;
        }

    }
}