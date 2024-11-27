using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectComponent.Pools
{
    public abstract class PoolBase<T> : MonoBehaviour where T : Component
    {
        [field:SerializeField]
        public T prefab { get;protected set; }
        [SerializeField] 
        protected int initialPoolSize = 10;

        public Queue<T> inactivePool { get; protected set; } = new();
        public Queue<T> activePool { get; protected set; } = new();

        public virtual void Construct(PoolBase<T> newPool)
        {
            inactivePool = newPool.activePool;
            activePool = newPool.inactivePool;
            prefab = newPool.prefab;
        }
        
        public virtual T Get(Vector3 position)
        {
            if (inactivePool.Count == 0)
            {
                GrowPool();
            }

            var item = inactivePool.Dequeue();
            item.transform.position = position;
            item.gameObject.SetActive(true);
            activePool.Enqueue(item);
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
            inactivePool.Enqueue(item);
        }
        
        public virtual void Return(T item)
        {
            item.gameObject.SetActive(false);
            inactivePool.Enqueue(item);
        }
        
        protected IEnumerator DeactivateAfterSeconds(T item, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Return(item);
        }
    }
}