using UnityEngine;

namespace GameObjectComponent.App
{
    public abstract class PersistentComponent : MonoBehaviour
    {
        [SerializeField]
        protected int id = 0;
        
        public abstract void Save();
        public abstract void Load();
        public abstract void Reset();
        
        private void OnEnable()
        {
            Load();
        }
        
        private void OnDisable()
        {
            Save();
        }
    }
    
    
}