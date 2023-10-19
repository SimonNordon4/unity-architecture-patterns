using UnityEngine;

namespace Classic.Pools
{
    public class ParticlePool : PoolBase<ParticleSystem>
    {
        public void GetForParticleDuration(Vector3 position)
        {
            base.GetForSeconds(position, prefab.main.duration);
        }
    }
}