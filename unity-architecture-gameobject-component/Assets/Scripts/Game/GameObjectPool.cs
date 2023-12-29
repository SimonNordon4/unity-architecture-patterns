using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectComponent.Game
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialPoolSize = 10;
        private Queue<GameObject> _pool = new Queue<GameObject>();
        
        private void Awake()
        {
            for (var i = 0; i < initialPoolSize; i++)
            {
                var obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }
        
        public GameObject Get(Vector3 position = new())
        {
            if (_pool.Count == 0)
            {
                var obj = Instantiate(prefab, position, Quaternion.identity, null);
                obj.SetActive(false);
                return obj;
            }

            var pooledObject = _pool.Dequeue();
            pooledObject.transform.position = position;
            pooledObject.SetActive(true);
            return pooledObject;
        }
        
        public GameObject GetForSeconds(Vector3 position, float seconds)
        {
            var obj = Get(position);
            StartCoroutine(ReturnAfterSeconds(obj, seconds));
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
        
        private IEnumerator ReturnAfterSeconds(GameObject obj, float seconds)
        {
            var time = 0f;
            while (time < seconds)
            {
                time += GameTime.deltaTime;
                yield return null;
            }
            Return(obj);
        }
    }
}