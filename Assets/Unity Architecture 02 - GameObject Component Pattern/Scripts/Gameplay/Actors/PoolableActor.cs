using UnityEngine;
using UnityEngine.Events;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class PoolableActor : MonoBehaviour
    {
        private ActorPool _pool;
        
        public UnityEvent onActorGet = new();
        public UnityEvent onActorReturn = new();

        public void Construct(ActorPool pool)
        {
            _pool = pool;
        }

        public void Return()
        {
            _pool.Return(this);
        }
    }
}