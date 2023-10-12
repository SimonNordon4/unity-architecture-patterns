using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyDefinition testDefinition;
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private Level level;
        
        public void SpawnEnemy(EnemyDefinition definition)
        {
            var position = GetRandomPosition();
            factory.Create(definition, position);
        }
        
        private Vector3 GetRandomPosition()
        {
            Vector3 randomInnerPoint = new Vector3(
                UnityEngine.Random.Range(-level.bounds.x, level.bounds.x),
                0,
                UnityEngine.Random.Range(-level.bounds.y, level.bounds.y)
            );

            Vector3 randomEdgePoint;
            float edgeSelection = UnityEngine.Random.value;
            if (edgeSelection < 0.25f)
                randomEdgePoint = new Vector3(-level.bounds.x, 0, UnityEngine.Random.Range(-level.bounds.y, level.bounds.y));
            else if (edgeSelection < 0.5f)
                randomEdgePoint = new Vector3(level.bounds.x, 0, UnityEngine.Random.Range(-level.bounds.y, level.bounds.y));
            else if (edgeSelection < 0.75f)
                randomEdgePoint = new Vector3(UnityEngine.Random.Range(-level.bounds.x, level.bounds.x), 0, -level.bounds.y);
            else
                randomEdgePoint = new Vector3(UnityEngine.Random.Range(-level.bounds.x, level.bounds.x), 0, level.bounds.y);

            float bias = 0.7f;  // Adjust this value to control the bias towards the edge. 1.0f is full edge, 0.0f is no bias.
            return Vector3.Lerp(randomInnerPoint, randomEdgePoint, bias);
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(EnemySpawner))]
    public class EnemySpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();  // Draw the default inspector for EnemySpawner

            var enemySpawner = (EnemySpawner)target;
            if (GUILayout.Button("Spawn Test Enemy"))
            {
                if (enemySpawner.testDefinition == null)
                {
                    Debug.LogError("Test Definition is not set.");
                    return;
                }

                enemySpawner.SpawnEnemy(enemySpawner.testDefinition);
            }
        }
    }
    #endif
}