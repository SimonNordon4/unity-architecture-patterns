using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classic.Pools
{
    public abstract class PoolBase<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        protected T prefab;
        [SerializeField] 
        protected int initialPoolSize = 10;
        
        protected readonly Queue<T> InactivePool = new();
        protected readonly Queue<T> ActivePool = new();
        
        public virtual T Get(Vector3 position)
        {
            if (InactivePool.Count == 0)
            {
                GrowPool();
            }

            var item = InactivePool.Dequeue();
            item.transform.position = position;
            item.gameObject.SetActive(true);
            ActivePool.Enqueue(item);
            return item;
        }
        
        public virtual T GetForSeconds(Vector3 position, float seconds)
        {
            var item = Get(position);
            StartCoroutine(DeactivateAfterSeconds(item, seconds));
            return item;
        }

        protected virtual void GrowPool()
        {
            var item = Instantiate(prefab, null);
            item.gameObject.SetActive(false);
            InactivePool.Enqueue(item);
        }
        
        public virtual void Return(T item)
        {
            item.gameObject.SetActive(false);
            InactivePool.Enqueue(item);
        }
        
        protected IEnumerator DeactivateAfterSeconds(T item, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Return(item);
        }
    }
}