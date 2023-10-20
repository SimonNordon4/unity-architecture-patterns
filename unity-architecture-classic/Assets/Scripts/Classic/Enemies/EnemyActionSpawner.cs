using Classic.Game;
using Definitions;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyActionSpawner : MonoBehaviour
    {
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private Level level;
        
        public void SpawnAction(SpawnAction action)
        {
            var position = GetRandomPosition();
            factory.Create(action.definition, position);
        }
        
        private Vector3 GetRandomPosition()
        {
            Vector3 randomInnerPoint = new Vector3(
                Random.Range(-level.bounds.x, level.bounds.x),
                0,
                Random.Range(-level.bounds.y, level.bounds.y)
            );

            var edgeSelection = UnityEngine.Random.value;
            var randomEdgePoint = edgeSelection switch
            {
                < 0.25f => new Vector3(-level.bounds.x, 0, Random.Range(-level.bounds.y, level.bounds.y)),
                < 0.5f => new Vector3(level.bounds.x, 0, Random.Range(-level.bounds.y, level.bounds.y)),
                < 0.75f => new Vector3(Random.Range(-level.bounds.x, level.bounds.x), 0, -level.bounds.y),
                _ => new Vector3(Random.Range(-level.bounds.x, level.bounds.x), 0, level.bounds.y)
            };

            float bias = 0.7f;  // Adjust this value to control the bias towards the edge. 1.0f is full edge, 0.0f is no bias.
            return Vector3.Lerp(randomInnerPoint, randomEdgePoint, bias);
        }
    }
}