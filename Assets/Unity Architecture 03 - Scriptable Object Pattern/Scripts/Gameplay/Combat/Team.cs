using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class Team : MonoBehaviour
    {
        public void ChangeTeam(int layer)
        {
            gameObject.layer = layer;
        }
    }
}