using UnityEngine;

namespace Classic.Core
{
    public class Level : MonoBehaviour
    {
        [field:SerializeField] public Vector2 bounds { get; private set; } = new(17.5f, 17.5f);
    }
}