using System.Collections.Generic;
using System.Linq;
using GameplayComponents.Combat;
using GameObjectComponent.Game;
using UnityEngine;

namespace GameObjectComponent.Items
{
    public class HealthPackSpawner : MonoBehaviour
    {
        // [SerializeField] private ActorStats stats;
        // [SerializeField] private HealthPackController healthPackPrefab;
        // [SerializeField] private GameState gameState;
        //
        // private readonly List<HealthPackController> _healthPacks = new();
        //
        // private void OnEnable()
        // {
        //     gameState.OnGameQuit+=(DestroyAllHealthPacks);
        // }
        //
        // private void OnDisable()
        // {
        //     gameState.OnGameQuit-=(DestroyAllHealthPacks);
        // }
        //
        // public void SpawnHealthPack(Vector3 position)
        // {
        //     var randomChance = Random.Range(0, 100);
        //     if (!(randomChance < stats.Map[StatType.HealthPackDropRate].value)) return;
        //     
        //     var pos = new Vector3(position.x, 0f, position.z);
        //     var healthPack = Instantiate(healthPackPrefab, pos, Quaternion.identity);
        //     healthPack.gameState = gameState;
        //     _healthPacks.Add(healthPack);
        // }
        //
        // public void DestroyAllHealthPacks()
        // {
        //     foreach (var healthPack in _healthPacks.Where(healthPack => healthPack != null))
        //     {
        //         Destroy(healthPack.gameObject);
        //     }
        //     _healthPacks.Clear();
        // }
    }
}