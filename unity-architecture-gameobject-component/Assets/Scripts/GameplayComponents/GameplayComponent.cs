using UnityEngine;

namespace GameplayComponents
{
    public abstract class GameplayComponent : MonoBehaviour
    {
        public virtual void Initialize()
        {
            
        }
        public virtual void Reinitialize()
        {
                
        }

        public virtual void Deinitialize()
        {
            
        }
    }
}