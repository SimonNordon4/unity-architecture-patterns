using UnityEngine;

namespace GameplayComponents.Combat
{
    [CreateAssetMenu(fileName = "MunitionDefinition", menuName = "Classic/Munition Definition")]
    public class MunitionDefinition : ScriptableObject
    {
        public Munition prefab;
    }
}