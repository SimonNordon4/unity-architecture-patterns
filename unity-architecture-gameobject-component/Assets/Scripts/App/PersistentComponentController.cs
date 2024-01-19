using UnityEngine;

namespace GameObjectComponent.App
{
    public class PersistentComponentController : MonoBehaviour
    {
        [SerializeField]
        private PersistentComponent[] persistentComponents;

        [SerializeField] private bool resetOnStart = false;

        public void Awake()
        {
            if (resetOnStart)
            {
                Reset();
            }
        }

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