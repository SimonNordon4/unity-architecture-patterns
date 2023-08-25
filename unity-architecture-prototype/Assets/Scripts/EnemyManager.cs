using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    
    [Header("Prefabs")]
    public List<EnemySpawnBlock> enemySpawnBlocks = new();
    private EnemySpawnBlock _currentBlock;
    public GameObject spawnIndicatorPrefab;
    public Transform playerTarget;

    private bool _blocksCompleted = false;

    [Header("Stats")] 
    public float spawnRadius = 10f;

    
    
    [Header("Enemies")]
    public List<GameObject> enemies = new List<GameObject>();
    

    // Will destroy all alive enemies.
    public void ResetEnemyManager()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
        StopAllCoroutines();
        foreach (var spawnBlock in enemySpawnBlocks)
        {
            spawnBlock.Start();
        }
        _blocksCompleted = false;
        _currentBlock = enemySpawnBlocks[0];
        
    }

    private void Start()
    {
        foreach (var spawnBlock in enemySpawnBlocks)
        {
            spawnBlock.Start();
        }
        _currentBlock = enemySpawnBlocks[0];
        
        StopAllCoroutines();
    }

    private void Update()
    {
        if (GameManager.instance.isGameActive == false) return;

        if (!_blocksCompleted)
        {
            _currentBlock.Update();
        }
        else
        {
            if(enemies.Count <= 0)
                gameManager.WinGame();
        }

    }

    private IEnumerator IndicateSpawn(GameObject enemyPrefab)
    {
        // select a random point on the circle
        var randomPoint = Random.insideUnitSphere.normalized * spawnRadius;
        randomPoint.y = enemyPrefab.transform.localScale.y;
        var spawnIndicator = Instantiate(spawnIndicatorPrefab, randomPoint, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        
        // Suspend the coroutine until the game is active
        while (GameManager.instance.isGameActive == false)
        {
            yield return null;
        }
        
        // Check if spawnIndicator still exists, if it was destroyed abort the spawn
        if (spawnIndicator == null)
        {
            yield break;
        }
        
        SpawnEnemy(enemyPrefab, randomPoint);
        Destroy(spawnIndicator);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        var newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().playerTarget = playerTarget;
        newEnemy.GetComponent<EnemyController>().enemyManager = this;
        enemies.Add(newEnemy);
    }

    public void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public void NextBlock()
    {
        var currentIndex = enemySpawnBlocks.IndexOf(_currentBlock);

        if (currentIndex + 1 >= enemySpawnBlocks.Count)
        {
            _blocksCompleted = true;
            return;
        }
        
        _currentBlock = enemySpawnBlocks[currentIndex + 1];
    }

    [Serializable]
    public class EnemySpawnBlock
    {
        public EnemyManager enemyManager;
        public int totalEnemies = 100;
        private int _spawnedEnemies = 0;
        private float[] spawnTimings;
        public AnimationCurve spawnRateCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        
        public List<EnemySpawnChance> enemySpawnChances = new();
        public int[] eliteSpawnTimings = {120};
        public List<GameObject> eliteEnemies = new();
        public GameObject bossEnemy;
        
        public float blockTime = 300f;
        private float _elapsedTime;

        private void GetSpawnTimings()
        {
            // Get the spawn timing of each enemy evaluated against the animation curve
            spawnTimings = new float[totalEnemies];
            for (int i = 0; i < totalEnemies; i++)
            {
                spawnTimings[i] = spawnRateCurve.Evaluate((float) i / totalEnemies) * blockTime;
            }
        }
        
        public void Start()
        {
            _elapsedTime = 0f;
            _spawnedEnemies = 0;
            // There's a issue if the block time is less than 2 seconds, because enemies only spawn after 1 second.
            // This means the block with finish before the first enemy spawns, instantly moving to the next block.
            // Or winning the game.
            blockTime = blockTime < 2 ? 2 : blockTime;
            GetSpawnTimings();
        }

        public void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_spawnedEnemies < totalEnemies)
            {
                if (_elapsedTime > spawnTimings[_spawnedEnemies])
                {
                    // Spawn one the enemies inside the spawn chances based on their spawn chance
                    var totalSpawnChance = 0;
                    foreach (var enemySpawnChance in enemySpawnChances)
                    {
                        totalSpawnChance += enemySpawnChance.spawnChance;
                    }
                
                    var randomSpawnChance = Random.Range(0, totalSpawnChance);
                    var currentSpawnChance = 0;
                
                    for (var i = 0; i < enemySpawnChances.Count; i++)
                    {
                        var x = i;
                        currentSpawnChance += enemySpawnChances[x].spawnChance;

                        if (randomSpawnChance >= currentSpawnChance)
                        {
                            Debug.Log("Less than spawn chance, skipping");
                            continue;
                        }
                        enemyManager.StartCoroutine(enemyManager.IndicateSpawn(enemySpawnChances[x].enemyPrefab));
                        Debug.Log("Spawning Enemy");
                        _spawnedEnemies++;
                        break;
                    }
                }
            }
            

            // for (var i = 0; i < eliteSpawnTimings.Length; i++)
            // {
            //     if(Mathf.FloorToInt(_elapsedTime) % eliteSpawnTimings[i] == 0)
            //     {
            //         // Spawn Elite
            //     }
            // }
            
            if(_spawnedEnemies >= totalEnemies)
            {
                // Spawn Boss
                Debug.Log("Spawning Boss");
                enemyManager.NextBlock();
            }
        }
    }

    [Serializable]
    public struct EnemySpawnChance
    {
        public GameObject enemyPrefab;
        public int spawnChance;
    }
}