using UnityEngine;

namespace Classic.Pools
{
    public class ParticlePool : PoolBase<ParticleSystem>
    {
        public ParticleSystem GetForParticleDuration(Vector3 position)
        {
            return base.GetForSeconds(position, prefab.main.duration);
        }
        public ParticleSystem GetForParticleDuration(Vector3 position, Color color)
        {
            var ps = base.GetForSeconds(position, prefab.main.duration);
            var main = ps.main;
            main.startColor = color;
            return ps;
        }
    }
}