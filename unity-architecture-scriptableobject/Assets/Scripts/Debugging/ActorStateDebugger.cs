using GameplayComponents.Combat;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameObjectComponent.Debugging
{
    [RequireComponent(typeof(GameplayStateController))]
    public class ActorStateDebugger : DebugComponent
    {
        private GameplayStateController _gameplayStateController;
        
        private void Awake()
        {
            _gameplayStateController = GetComponent<GameplayStateController>();
        }
        
        private void OnEnable()
        {

        }
    }
}