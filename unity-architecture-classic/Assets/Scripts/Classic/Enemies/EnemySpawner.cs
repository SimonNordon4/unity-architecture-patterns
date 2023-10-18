using Classic.Game;
using Classic.Items;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private SpawnIndicatorController spawnIndicator;
        [SerializeField] private ParticlePool particlePool;

        public void SpawnEnemy(EnemyDefinition definition)
        { 
            // var position = GetRandomPosition();
            // var indicator = Instantiate(indicatorPrefab, position, Quaternion.identity, null);
            // // indicator.SetSize(definition.spawnIndicatorSize);
            // var psColor = indicator.GetComponentInChildren<ParticleSystemColor>();
            // if (psColor != null)
            // {
            //     psColor.SetColor(definition.enemyColor);
            // }
            //
            // // Subscribe to the OnCompleted and OnCancelled events of the indicator
            // indicator.OnCompleted += () => OnIndicatorCompleted(definition, position);
        }

        private void OnIndicatorCompleted(EnemyDefinition definition, Vector3 position)
        {
            var enemy = factory.Create(definition, position);
            SpawnDeathParticle(definition, position);
        }

        public void SpawnDeathParticle(EnemyDefinition definition, Vector3 position)
        {
            // Play the particle effect
            var particle = particlePool.Get(position);
            if(particle.TryGetComponent<ParticleSystemColor>(out var psColor))
            {
                psColor.SetColor(definition.enemyColor);
            }
        }
        
        private Vector3 GetRandomPosition()
        {
            Vector3 randomInnerPoint = new Vector3(
                Random.Range(-level.bounds.x, level.bounds.x),
                0,
                Random.Range(-level.bounds.y, level.bounds.y)
            );

            Vector3 randomEdgePoint;
            var edgeSelection = UnityEngine.Random.value;
            randomEdgePoint = edgeSelection switch
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