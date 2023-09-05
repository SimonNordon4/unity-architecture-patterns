using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class ProgressionManagerUI : MonoBehaviour
{
    public RectTransform progressionContainer;
    public RectTransform currentProgression;
    public RectTransform blockMarker;
    public RectTransform progressTrail;
    
    public RectTransform blockContainer;

    private float _containerHeight;
    private float _progressIncrement;
    private float _blockIncrement;
    private int _numberOfBlocks;

    public Color defaultColor;
    public Color progressedColor;

    private List<RectTransform> _blockMarkers = new();
    private BlockProgressData[] _blocks;
    
    private EnemyManager _enemyManager;
    private float _currentMarketHeight = 0;
    private int _currentBlockIndex = 0;
    private int _currentWaveIndex = 0;

    private void OnEnable()
    {
        currentProgression.GetComponent<Image>().color = progressedColor;
        progressTrail.GetComponent<Image>().color = progressedColor;
        
        _containerHeight = progressionContainer.rect.height;
        
        // get total number of blocks
        _enemyManager = FindObjectOfType<EnemyManager>();

        if (_enemyManager == null) return;
        
        _numberOfBlocks = _enemyManager.enemySpawnRound.enemySpawnBlocks.Count;
        _blockIncrement = _containerHeight / _numberOfBlocks;
        
        _blocks = new BlockProgressData[_numberOfBlocks];

        // Initialization Loop.
        for (var i = 0; i < _numberOfBlocks; i++)
        {
            var newBlock = new BlockProgressData();
            newBlock.waves = _enemyManager.enemySpawnRound.enemySpawnBlocks[i].spawnWaves.Count;
            newBlock.enemyWaveCount = new int[newBlock.waves];
            _blocks[i] = newBlock;
        }

        var lastBlockCount = 0;
        for(var i = 0; i < _numberOfBlocks; i++)
        {
            var block = _enemyManager.enemySpawnRound.enemySpawnBlocks[i];
            var data = new BlockProgressData
            {
                enemyBlockCount = block.TotalEnemyCount() + lastBlockCount
            };

            lastBlockCount = data.enemyBlockCount;
            
            // We also want to instantiate a block marker for each block.
            var blockMarkerInstance = Instantiate(blockMarker, blockContainer);
            data.blockHeight = _blockIncrement * (i + 1);
            blockMarkerInstance.anchoredPosition = new Vector2(0,  data.blockHeight);
            blockMarkerInstance.gameObject.SetActive(true);
            blockMarkerInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}";
            
            var blockAmountReached = data.enemyBlockCount <= _enemyManager.totalEnemiesKilled;
            blockMarkerInstance.GetComponent<Image>().color = blockAmountReached ? progressedColor : defaultColor;
            _blockMarkers.Add(blockMarkerInstance);
            _blocks[i] = data;
        }
        
        blockMarker.gameObject.SetActive(false);
        
        _progressIncrement = _blockIncrement / _blocks[0].enemyBlockCount;
        
        currentProgression.gameObject.SetActive(true);
        progressTrail.gameObject.SetActive(true);
        
        
        currentProgression.anchoredPosition = new Vector2(0, _currentMarketHeight);
        progressTrail.sizeDelta = new Vector2(progressTrail.sizeDelta.x, _currentMarketHeight);
        
    }
    private void OnDisable()
    {
        foreach(var blockData in _blocks)
        {
            foreach (var marker in blockData.waveMarkers)
            {
                Destroy(marker.gameObject);
            }
        }
        
        foreach (var marker in _blockMarkers)
        {
            Destroy(marker.gameObject);
        }
        _blockMarkers.Clear();
        
        _currentMarketHeight = currentProgression.anchoredPosition.y;
        
        progressTrail.sizeDelta = new Vector2(progressTrail.sizeDelta.x, 0);
        progressTrail.gameObject.SetActive(false);
        
        currentProgression.anchoredPosition = new Vector2(0, 0);
        currentProgression.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        var currentProgressionHeight = Mathf.Lerp(currentProgression.anchoredPosition.y, _progressIncrement * _enemyManager.totalEnemiesKilled, Time.deltaTime); 
        currentProgression.anchoredPosition = new Vector2(0, currentProgressionHeight);
        progressTrail.sizeDelta = new Vector2(progressTrail.sizeDelta.x, currentProgressionHeight);
        
        // check if the currentProgressionHeight is greater than the current block marker.
        if (_enemyManager.totalEnemiesKilled >= _blocks[_currentBlockIndex].enemyBlockCount)
        {
            _blockMarkers[_currentBlockIndex].GetComponent<Image>().color = progressedColor;
            _currentWaveIndex = 0;
            if (_currentBlockIndex + 1 < _blocks.Length)
            {
                _currentBlockIndex++;
            }
            
            _progressIncrement = _blockIncrement / (_blocks[_currentBlockIndex].enemyBlockCount - _blocks[_currentBlockIndex - 1].enemyBlockCount);
        }


    }

    [Serializable]
    public class BlockProgressData
    {
        public float blockHeight;
        public int enemyBlockCount;
        public int waves;
        public int[] enemyWaveCount;
        public List<RectTransform> waveMarkers = new();


    }
}
