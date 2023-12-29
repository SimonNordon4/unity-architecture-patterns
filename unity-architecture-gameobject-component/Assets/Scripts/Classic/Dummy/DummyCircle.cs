using UnityEngine;

namespace Classic.Dummy
{
    public class DummyCircle : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float radius = 1f;
        private float _angle = 0f;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _angle += speed * Time.deltaTime; // Change the angle
            var x = Mathf.Cos(_angle) * radius;
            var y = Mathf.Sin(_angle) * radius;
            _transform.position = new Vector3(x, _transform.position.y, y);
        }
    }
}