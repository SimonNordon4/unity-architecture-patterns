using Classic.Game;
using UnityEngine;

namespace Classic.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {

        private Transform _transform;
        public Vector3 moveDirection { get; set; }
        public Vector3 lookDirection { get; set; }
        public float speed { get; set; }

        private void Start()
        {
            _transform = transform;
        }
        
        private void LateUpdate()
        {
            if (moveDirection.magnitude > 0.01f)
            {
                _transform.position += moveDirection * (speed * GameTime.deltaTime);
            }

            if (lookDirection.magnitude > 0.01f)
            {
                _transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}