
using UnityEngine;
namespace UnityArchitecture.GameObjectComponentPattern
{
    public class HealthPack : MonoBehaviour
    {
        private float _lifeTime = 5f;
        private float _aliveTime = 0f;

        // Update is called once per frame
        void Update()
        {
            _aliveTime += Time.deltaTime;
            if (_aliveTime > _lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
