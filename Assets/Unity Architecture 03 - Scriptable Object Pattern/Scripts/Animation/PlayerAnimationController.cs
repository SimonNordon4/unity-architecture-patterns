using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]
        private CombatTarget combatTarget;
        private UnityEngine.Transform _transformToFollow;

        [SerializeField]
        private UnityEngine.Transform gunPivot;

        private Vector3 offset;
        private UnityEngine.Transform _transform;

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
            var gunRotation = Quaternion.LookRotation(combatTarget.TargetDirection);
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, gunRotation, Time.deltaTime * gunRotationSpeed);
            _transform.position = _transformToFollow.position + offset;
            _transform.rotation = Quaternion.Lerp(_transform.rotation, _transformToFollow.rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
