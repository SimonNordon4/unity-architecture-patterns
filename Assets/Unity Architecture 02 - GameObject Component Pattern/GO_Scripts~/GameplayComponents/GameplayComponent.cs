using GameObjectComponent.Game;
using GameplayComponents.Actor;
using UnityEngine;

namespace GameplayComponents
{
    public abstract class GameplayComponent : MonoBehaviour
    {
        public virtual void OnGameStart()
        {
            
        }
        
        public virtual void OnGameEnd()
        {
            
        }
    }
}