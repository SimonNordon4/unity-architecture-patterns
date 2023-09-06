using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The progress bas is evenly divided into each block.
// Each block is evenly divided into each wave.
// Each time an enemy is killed, the progress bar is incremented by the wave height * waveProgress.

[DefaultExecutionOrder(100)]
public class ProgressionManagerUI : MonoBehaviour
{
    [Header("UI")]
    public RectTransform progressionContainer;
    public RectTransform progressMarker;
    public RectTransform progressTrail;
    public RectTransform blockContainer;
    public RectTransform blockMarker;
    public RectTransform waveContainer;
    public RectTransform waveMarker;
    
    [Header("Notification")]
    public GameObject notificationObject;
    public TextMeshProUGUI notificationText;
    
    [Header("Colors")]
    public Color defaultColor;
    public Color progressedColor;
    
    private EnemyManager _enemyManager;
    private BlockProgressData[] _blocks;
    
    private BlockProgressData _currentBlock;
    private float _lastMarkerHeight;

    private void Awake()
    {
        // Normalize colors for permanent UI elements.
        progressMarker.GetComponent<Image>().color = progressedColor;
        progressTrail.GetComponent<Image>().color = progressedColor;
    }

    private void OnEnable()
    {
        // Validate.
        _enemyManager = FindObjectOfType<EnemyManager>();
        if (_enemyManager == null) return;
        
        #region Build Data
        var numberOfBlocks = _enemyManager.enemySpawnRound.enemySpawnBlocks.Count;
        var blockIncrement = progressionContainer.rect.height / numberOfBlocks;
        _blocks = new BlockProgressData[numberOfBlocks];

        float previousBlockPosition = 0;
        int lastWaveEnemyCount = 0;
        for (var i = 0; i < numberOfBlocks; i++)
        {
            var waveCount = _enemyManager.enemySpawnRound.enemySpawnBlocks[i].spawnWaves.Count;
            var newBlockMarker =Instantiate(blockMarker, blockContainer);
            newBlockMarker.gameObject.SetActive(true);
            
            var blockData = new BlockProgressData
            {
                blockMarker = newBlockMarker,
                blockMarkerImage = newBlockMarker.GetComponent<Image>(),
                blockMarkerPosition = (i + 1) * blockIncrement,
                number = i + 1,
                waveHeight = blockIncrement / waveCount,
                waveCount = waveCount,
                waves = new WaveProgressData[waveCount]
            };

            for (var j = 0; j < blockData.waveCount; j++)
            {
                var waveEnemyCount = _enemyManager.enemySpawnRound.enemySpawnBlocks[i].spawnWaves[j].TotalEnemyCount();
                var newWaveMarker = Instantiate(waveMarker, waveContainer);
                newWaveMarker.gameObject.SetActive(true);
                
                var waveData = new WaveProgressData
                {
                    number = j + 1,
                    waveMarker = newWaveMarker,
                    waveMarkerImage = newWaveMarker.GetComponent<Image>(),
                    waveMarkerPosition = (j + 1) * blockData.waveHeight + previousBlockPosition,
                    waveHeight = blockData.waveHeight,
                    enemiesInWave = waveEnemyCount,
                    enemiesUpToWave = waveEnemyCount + lastWaveEnemyCount,
                    waveProgress = 0
                };
                
                lastWaveEnemyCount += waveEnemyCount;
                
                blockData.waves[j] = waveData;
            }
            
            previousBlockPosition += blockIncrement;
            
            _blocks[i] = blockData;
        }
        #endregion
        
        // Active objects to be instantiated.
        blockMarker.gameObject.SetActive(true);
        waveMarker.gameObject.SetActive(true);
        
        #region Build UI

        var blocksCompleted = 0;
        var wavesCompletedInBlock = 0;
        foreach (var block in _blocks)
        {
            foreach (var wave in block.waves)
            {
                wave.waveMarker.anchoredPosition = new Vector2(0, wave.waveMarkerPosition);
                var isWaveCompleted = wave.enemiesUpToWave <= _enemyManager.totalEnemiesKilled;
                wave.waveMarkerImage.color = isWaveCompleted ? progressedColor : defaultColor;

                if (isWaveCompleted)
                {
                    wavesCompletedInBlock++;
                }
            }
            
            block.blockMarker.anchoredPosition = new Vector2(0, block.blockMarkerPosition);
            block.blockMarker.GetComponentInChildren<TextMeshProUGUI>().text = $"{block.number}";
            var isBlockCompleted = block.waves[^1].enemiesUpToWave <= _enemyManager.totalEnemiesKilled;
            block.blockMarkerImage.color = isBlockCompleted ? progressedColor : defaultColor;

            if (isBlockCompleted)
            {
                blocksCompleted++;
                wavesCompletedInBlock = 0;
            }
        }

        var progressHeight = _blocks[^1].blockMarkerPosition;
        // We only set the blocks and waves if there are any left to complete.
        if(blocksCompleted < _blocks.Length)
        {
            _currentBlock = _blocks[blocksCompleted];
            var currentWave = _currentBlock.waves[wavesCompletedInBlock];

            if (currentWave == null)
            {
                throw new Exception("Current wave is null");
            }
        
            currentWave.waveProgress =
                (float)(currentWave.enemiesUpToWave - _enemyManager.totalEnemiesKilled) / currentWave.enemiesInWave;
        
            progressHeight = currentWave.waveMarkerPosition + currentWave.waveProgress * currentWave.waveHeight;
        
            _currentBlock.currentWave = currentWave;
        }
        
        #endregion
        
        // Update Progress.
        
        progressMarker.anchoredPosition = new Vector2(0, progressHeight);
        progressTrail.sizeDelta = new Vector2(progressTrail.sizeDelta.x, progressHeight);
        
        progressMarker.gameObject.SetActive(true);
        progressTrail.gameObject.SetActive(true);
        
        // Clean Up.
        blockMarker.gameObject.SetActive(false);
        waveMarker.gameObject.SetActive(false);
    }
    

    void LateUpdate()
    {
        MoveProgressBar();
        CheckWaveProgression();
    }

    private void MoveProgressBar()
    {
        // First we need to get the progress of the current wave every frame.
        // First we take the total enemies killed and subtract the enemies up to the current wave,
        var enemiesKilledSinceLastWave = _enemyManager.totalEnemiesKilled - _currentBlock.currentWave.enemiesUpToWave;
        // Now we get the proportion of the current wave that has been completed.
        var waveProgress = (float)enemiesKilledSinceLastWave / _currentBlock.currentWave.enemiesInWave;
        // now we can extrapolate the current progress.
        var progressHeight = _currentBlock.currentWave.waveMarkerPosition + waveProgress * _currentBlock.currentWave.waveHeight;
        // Apply to UI.
        
        progressMarker.anchoredPosition = new Vector2(0, progressHeight);
        progressTrail.sizeDelta = new Vector2(progressTrail.sizeDelta.x, progressHeight);
    }


    private void CheckWaveProgression()
    {
        // The wave is finished when the total enemies killed is greater than the enemies up to the current wave.
        if (_enemyManager.totalEnemiesKilled < _currentBlock.currentWave.enemiesUpToWave) return;
        
        // Set the wave marker color to completed.
        _currentBlock.currentWave.waveMarkerImage.color = progressedColor;
        
        // Now we must increment the wave.
        
        // If we're not the last wave in the block, simply move to the next one.
        if (_currentBlock.currentWave.number < _currentBlock.waveCount)
        {
            _currentBlock.currentWave = _currentBlock.waves[_currentBlock.currentWave.number];
            return;
        }
        
        // If we are the last wave in the block, then that means the block has been completed at this point.
        FinishCurrentBlock();

    }
    
    private void FinishCurrentBlock()
    {
        // Set the block marker color to completed.
        _currentBlock.blockMarkerImage.color = progressedColor;
        
        // If we're the last block, then do nothing return.
        // This will repeat every frame until the game ends.
        if (_currentBlock.number == _blocks.Length) return;
        
        // Otherwise, move to the next block.
        _currentBlock = _blocks[_currentBlock.number];
        _currentBlock.currentWave = _currentBlock.waves[0];

        GameManager.instance.StartCoroutine(ShowProgressNotification());
    }

    private IEnumerator ShowProgressNotification()
    {
        var currentWave = _enemyManager.GetNextWave();
        var healthIncrease = (currentWave.healthMultiplier.x + currentWave.healthMultiplier.y) * 0.5f;
        var damageIncrease = (currentWave.damageMultiplier.x + currentWave.damageMultiplier.y) * 0.5f;
        var newText = $"+{healthIncrease * 100}% enemy health\n{damageIncrease * 100}% enemy damage";
        notificationText.text = newText;
        notificationObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        notificationObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        // Clean up instantiated objects.
        foreach (var block in _blocks)
        {
            foreach (var wave in block.waves)
            {
                Destroy(wave.waveMarker.gameObject);
            }
            
            Destroy(block.blockMarker.gameObject);
        }
        
        _blocks = null;
        
        progressMarker.gameObject.SetActive(false);
        progressTrail.gameObject.SetActive(false);
    }

    [Serializable]
    public class BlockProgressData
    {
        public RectTransform blockMarker;
        public Image blockMarkerImage;
        public float blockMarkerPosition;
        public int number;

        public WaveProgressData currentWave;
        public float waveHeight;
        public int waveCount;
        public WaveProgressData[] waves;
    }

    [Serializable]
    public class WaveProgressData
    {
        public RectTransform waveMarker;
        public Image waveMarkerImage;
        
        public int number;
        public float waveMarkerPosition;
        public float waveHeight;
        public int enemiesInWave;
        public int enemiesUpToWave;
        public float waveProgress;
    }
}
