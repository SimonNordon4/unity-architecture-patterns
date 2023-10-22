using Classic.Actors;
using Classic.Game;
using Classic.Pools;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic.Enemies
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private GameState state;
        [SerializeField] private Level level;
        [field:SerializeField] public Transform initialTarget { get; private set; }
        [SerializeField] private ParticlePool deathParticlePool;

        public GameObject Create(EnemyDefinition enemyDefinition, Vector3 position = new(), bool startActive = true)
        {
            enemyDefinition.enemyPrefab.SetActive(false);
            var enemy = Instantiate(enemyDefinition.enemyPrefab, position, Quaternion.identity, null);

            if (enemy.TryGetComponent<ActorState>(out var gameState))
                gameState.Construct(state);

            if (enemy.TryGetComponent<ActorMovement>(out var movement))
                movement.Construct(level);

            if (enemy.TryGetComponent<ActorTarget>(out var target))
                target.SetTarget(initialTarget);

            if (enemy.TryGetComponent<ParticlePool>(out var particlePool))
                particlePool.Construct(deathParticlePool);
            
            enemy.SetActive(startActive);

            return enemy;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyFactory))]
    public class EnemyFactoryEditor : Editor
    {
        private EnemyDefinition enemyDefinition;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var factory = (EnemyFactory)target;

            enemyDefinition =
                (EnemyDefinition)EditorGUILayout.ObjectField(enemyDefinition, typeof(EnemyDefinition), false);

            if (GUILayout.Button("Create Enemy"))
            {
                factory.Create(enemyDefinition);
            }
        }

    }
#endif
}

