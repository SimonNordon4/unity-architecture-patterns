using UnityEngine;

namespace UnityArchitecture.GameObjectComponentPattern
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]
        private CombatTarget combatTarget;
        private Transform _transformToFollow;

        [SerializeField]
        private Transform gunPivot;

        private Vector3 offset;
        private Transform _transform;

        [SerializeField]
        private float rotationSpeed = 1f;
        [SerializeField]
        private float gunRotationSpeed = 1f;
        // Start is called before the first frame update
        void Start()
        {
            _transformToFollow = combatTarget.transform;
            _transform = transform;
            offset = transform.position - _transformToFollow.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!GameManager.Instance.isGameActive) return;
            var gunRotation = Quaternion.LookRotation(combatTarget.TargetDirection);
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, gunRotation, Time.deltaTime * gunRotationSpeed);
            _transform.position = _transformToFollow.position + offset;
            _transform.rotation = Quaternion.Lerp(_transform.rotation, _transformToFollow.rotation, Time.deltaTime * rotationSpeed);
        }
    }
}