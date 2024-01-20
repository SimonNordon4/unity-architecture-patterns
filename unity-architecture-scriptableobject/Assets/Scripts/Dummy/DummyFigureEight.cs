using UnityEngine;

namespace GameObjectComponent.Dummy
{
    public class DummyFigureEight : MonoBehaviour
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
            _angle += speed * Time.deltaTime;
            var x = Mathf.Sin(_angle) * radius;
            var y = Mathf.Sin(_angle * 2) * radius;
            _transform.position = new Vector3(x, _transform.position.y, y);
        }
    }
}