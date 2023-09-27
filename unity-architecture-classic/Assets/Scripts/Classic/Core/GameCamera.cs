using UnityEngine;

namespace Classic.Core
{
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
            if (characterTransform.position.x <= -level.bounds.x + edgeDistance ||
                characterTransform.position.x >= level.bounds.x - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(transform.position.x, cameraWishPosition.y, cameraWishPosition.z);
            }

            if (characterTransform.position.z <= -level.bounds.y + edgeDistance ||
                characterTransform.position.z >= level.bounds.y - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(cameraWishPosition.x, cameraWishPosition.y, transform.position.z);
            }

            transform.position = cameraWishPosition;
        }
    }
}