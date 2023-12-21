using System.Collections.Generic;
using Classic.Actors;
using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    [RequireComponent(typeof(EnemyRoundSpawner))]
    public class EnemyDirector : MonoBehaviour
    {
        [SerializeField]private GameState gameState;
        private EnemyRoundSpawner _enemyRoundSpawner;
        private HashSet<ActorComponent> _enemySpawnerComponents = new();

        private void Awake()
        {
            _enemySpawnerComponents = new HashSet<ActorComponent>(GetComponents<ActorComponent>());
        }

        private void OnEnable()
        {
            _enemyRoundSpawner = GetComponent<EnemyRoundSpawner>();
            gameState.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            foreach (var actorComponent in _enemySpawnerComponents)
            {
                actorComponent.Reset();
            }
            _enemyRoundSpawner.StartRoundSpawner();
        }
    }
}