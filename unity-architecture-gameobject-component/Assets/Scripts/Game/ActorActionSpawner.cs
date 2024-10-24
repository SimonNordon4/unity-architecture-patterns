﻿using System.Collections;
using GameObjectComponent.Definitions;
using GameplayComponents.Actor;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameObjectComponent.Game
{
    [RequireComponent(typeof(ActorPool))]
    public class ActorActionSpawner : MonoBehaviour
    {
        public ActorPool pool { get;private set; }
        [SerializeField] private Level level;

        private void Awake()
        {
            pool = GetComponent<ActorPool>();
        }

        public PoolableActor[] SpawnAction(SpawnActionDefinition actionDefinition)
        {
            return actionDefinition.actionType switch
            {
                (SpawnActionType.Group) => SpawnGroup(actionDefinition),
                (SpawnActionType.Encircle) => SpawnCircle(actionDefinition),
                _ => null
            };
        }

        private PoolableActor[] SpawnCircle(SpawnActionDefinition actionDefinition)
        {
            // Spawn the number of enemies in a circle around the player
            // Except if those enemies would be spawned outside of the level bounds, they are instead flipped to spawn on the other side of the circle
            var playerPosition = pool.factory.initialTarget.position;
            var radius = 5f;
            var angle = 360f / actionDefinition.numberOfEnemiesToSpawn;
            
            var enemies = new PoolableActor[actionDefinition.numberOfEnemiesToSpawn];
            for (int i = 0; i < actionDefinition.numberOfEnemiesToSpawn; i++)
            {
                var position = playerPosition + new Vector3(
                    Mathf.Cos(angle * i * Mathf.Deg2Rad) * radius,
                    0,
                    Mathf.Sin(angle * i * Mathf.Deg2Rad) * radius
                );
                
                // If the position is outside of the level bounds, we want to instead fit it inside the level bounds.

                // Correct x bounds
                if (Mathf.Abs(position.x) > level.bounds.x)
                {
                    position.x = playerPosition.x - (position.x - playerPosition.x);
                    if (Mathf.Abs(position.x) > level.bounds.x)
                    {
                        position.x = Mathf.Sign(position.x) * level.bounds.x;
                    }
                }

                // Correct z bounds
                if (Mathf.Abs(position.z) > level.bounds.y)
                {
                    position.z = playerPosition.z - (position.z - playerPosition.z);
                    if (Mathf.Abs(position.z) > level.bounds.y)
                    {
                        position.z = Mathf.Sign(position.z) * level.bounds.y;
                    }
                }
                
                enemies[i] = pool.Get(actionDefinition.definition, position);
            }

            return enemies;
        }

        private PoolableActor[] SpawnGroup(SpawnActionDefinition actionDefinition)
        {
            var enemies = new PoolableActor[actionDefinition.numberOfEnemiesToSpawn];
            // spawn the first enemy immediately
            var position = GetRandomPosition();
            enemies[0] = pool.Get(actionDefinition.definition, position);
            
            // spawn the rest of the enemies after a delay
            for (int i = 1; i < actionDefinition.numberOfEnemiesToSpawn; i++)
            {
                // make the position 1m away from the last position in a random direction
                var random = Random.insideUnitCircle;
                position = position + new Vector3(random.x, 0, random.y);
                enemies[i] = pool.Get(actionDefinition.definition, position, false);
                StartCoroutine(EnableEnemyAfterSeconds(enemies[i], i * 0.1f));
            }

            return enemies;
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
        
        private IEnumerator EnableEnemyAfterSeconds(PoolableActor poolableEnemy, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            poolableEnemy.gameObject.SetActive(true);
        }
    }
}