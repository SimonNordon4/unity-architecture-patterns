using UnityEngine;

namespace GameObjectComponent.App
{
    public class PersistentComponentController : MonoBehaviour
    {
        [SerializeField]
        private PersistentComponent[] persistentComponents;

        public void Reset()
        {
            foreach (var component in persistentComponents)
            {
                component.Reset();
            }
        }
        
        [ContextMenu("Get All Persistent Components")]
        public void GetAllPersistentComponents()
        {
            persistentComponents = FindObjectsOfType<PersistentComponent>();
        }
    }
}