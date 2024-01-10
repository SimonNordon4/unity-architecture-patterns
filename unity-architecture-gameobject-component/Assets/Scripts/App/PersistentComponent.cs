using UnityEngine;

namespace GameObjectComponent.App
{
    public abstract class PersistentComponent : MonoBehaviour
    {
        [SerializeField]
        protected int id = 0;
        
        public abstract void Save();
        public abstract void Load();
        
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