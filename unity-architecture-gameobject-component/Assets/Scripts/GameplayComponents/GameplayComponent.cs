using UnityEngine;

namespace GameplayComponents
{
    public abstract class GameplayComponent : MonoBehaviour
    {
        public virtual void Construct()
        {
            
        }
        public virtual void ReConstruct()
        {
                
        }

        public virtual void DeConstruct()
        {
            
        }
    }
}