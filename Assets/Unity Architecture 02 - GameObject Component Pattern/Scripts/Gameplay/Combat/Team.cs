using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class Team : MonoBehaviour
    {
        public void ChangeTeam(int layer)
        {
            gameObject.layer = layer;
        }
    }
}