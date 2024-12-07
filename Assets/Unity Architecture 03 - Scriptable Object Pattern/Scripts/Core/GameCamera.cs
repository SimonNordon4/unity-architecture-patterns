using System.Linq;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    /// <summary>
    /// Keeps the camera centered on the player, but stops the camera from moving if the player is within 5m of the level bounds.
    /// </summary>
    public class GameCamera : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Level level;


        [Header("Settings")]
        [SerializeField] private float edgeDistance = 5f;
        private Vector3 _cameraOffset;

        private void Start()
        {
            _cameraOffset = transform.position - characterTransform.position;
        }

        private void LateUpdate()
        {
            // Camera position.
            var cameraWishPosition = characterTransform.position + _cameraOffset;

            // We want the same level bound logic for the camera, but it stops its position if the player is within 5m of the level bounds
            if (characterTransform.position.x <= -level.Bounds.x + edgeDistance ||
                characterTransform.position.x >= level.Bounds.x - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(transform.position.x, cameraWishPosition.y, cameraWishPosition.z);
            }

            if (characterTransform.position.z <= -level.Bounds.y + edgeDistance ||
                characterTransform.position.z >= level.Bounds.y - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(cameraWishPosition.x, cameraWishPosition.y, transform.position.z);
            }

            transform.position = cameraWishPosition;
        }
    }
}