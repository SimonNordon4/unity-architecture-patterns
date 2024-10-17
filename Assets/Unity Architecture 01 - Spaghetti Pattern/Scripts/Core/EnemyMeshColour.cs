using UnityEngine;
namespace UnityArchitecture.SpaghettiPattern
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EnemyMeshColour : MonoBehaviour
    {
        public Color enemyColor = Color.red;

        public MeshRenderer body;
        public MeshRenderer nose;

        public void OnEnable()
        {
            body.material.color = enemyColor;
            nose.material.color = enemyColor;
        }
    }
}