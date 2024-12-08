using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    /// <summary>
    /// Keeps the camera centered on the player, but stops the camera from moving if the player is within 5m of the level bounds.
    /// </summary>
    public class GameCamera : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TransformData transformData;
        [SerializeField] private Level level;


        [Header("Settings")]
        [SerializeField] private float edgeDistance = 5f;
        private Vector3 _cameraOffset;
        private Transform _transformToFollow;

        private void Start()
        {
            if (transformData == null || transformData.IsNull)
            {
                Debug.LogWarning("No transform data provided.");
                return;
            }
            _transformToFollow = transformData.Data;
            
            _cameraOffset = transform.position - _transformToFollow.position;
        }

        private void LateUpdate()
        {
            // Camera position.
            var cameraWishPosition = _transformToFollow.position + _cameraOffset;

            // We want the same level bound logic for the camera, but it stops its position if the player is within 5m of the level bounds
            if (_transformToFollow.position.x <= -level.Bounds.x + edgeDistance ||
                _transformToFollow.position.x >= level.Bounds.x - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(transform.position.x, cameraWishPosition.y, cameraWishPosition.z);
            }

            if (_transformToFollow.position.z <= -level.Bounds.y + edgeDistance ||
                _transformToFollow.position.z >= level.Bounds.y - edgeDistance)
            {
                cameraWishPosition =
                    new Vector3(cameraWishPosition.x, cameraWishPosition.y, transform.position.z);
            }

            transform.position = cameraWishPosition;
        }
    }
}