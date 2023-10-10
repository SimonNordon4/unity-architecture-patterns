using System;
using Classic.Game;
using Definitions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Classic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyPool pool;
        [SerializeField] private Level level;
        public EnemyDefinition testDefinition;



        public void StartSpawnAction(EnemySpawnAction action)
        {
            var location = RandomSpawnLocation();
            
        }

        public void SpawnEnemy(EnemyDefinition definition, bool isBoss = false)
        {
            pool.Spawn(definition, RandomSpawnLocation(), isBoss);
        }

        private Vector3 RandomSpawnLocation()
        {
            // Generate a random angle in radians
            float angle = Random.Range(0f, 2f * Mathf.PI);

            // Generate a random distance from the center of the circle
            // Use the square root of the random value to weight the distribution towards the outside
            float distance = Mathf.Min(level.bounds.x,level.bounds.y) * Mathf.Sqrt(Random.value);

            // Calculate the x and y coordinates of the spawn location
            var x = distance * Mathf.Cos(angle);
            var y = distance * Mathf.Sin(angle);

            // Return the spawn location as a Vector3
            return new Vector3(x, 1f,y);
        }
    }
}