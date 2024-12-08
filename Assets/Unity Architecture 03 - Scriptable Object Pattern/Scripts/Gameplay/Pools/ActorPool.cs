using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class ActorPool : ScriptableObject
    {
        [SerializeField]
        private GameObject actorPrefab;

        private readonly Queue<GameObject> _inactivePool = new();

        public GameObject Get(Vector3 position, bool startActive = true)
        {
            var actor = _inactivePool.Count == 0 ?
                Instantiate(actorPrefab, position, Quaternion.identity) :
                _inactivePool.Dequeue();
            
            actor.gameObject.SetActive(startActive);
            return actor;
        }

        public void Return(GameObject actor)
        {
            actor.transform.position = Vector3.zero;
            actor.gameObject.SetActive(false);
            _inactivePool.Enqueue(actor);
        }
    }
}
