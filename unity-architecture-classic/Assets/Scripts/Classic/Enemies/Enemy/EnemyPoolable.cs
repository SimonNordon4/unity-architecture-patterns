using UnityEngine;

namespace Classic.Enemies.Enemy
{
    public class EnemyPoolable : MonoBehaviour
    {
        [SerializeField] private EnemyScope scope;
        public EnemyPool pool { private get; set; }

        public void Return()
        {
            pool.DeSpawn(scope);
        }
    }
}