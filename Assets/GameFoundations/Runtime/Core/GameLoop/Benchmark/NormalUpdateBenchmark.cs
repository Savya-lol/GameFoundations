using UnityEngine;

namespace Savya.GameFoundations.GameLoop.Benchmark
{
    public class NormalUpdateBenchmark : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 45f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private bool _doCalculations = true;

        private Vector3 _direction;
        private float _calculation;

        private void Start()
        {
            _direction = Random.insideUnitSphere;
        }

        private void Update()
        {
            // Rotation
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

            // Movement
            transform.position += _direction * _moveSpeed * Time.deltaTime;

            // Some calculations to simulate work
            if (_doCalculations)
            {
                _calculation = Mathf.Sin(Time.time) * Mathf.Cos(Time.time * 0.5f);
                _calculation *= Vector3.Distance(transform.position, Vector3.zero);
            }

            // Boundary check
            if (transform.position.magnitude > 100f)
            {
                _direction = -_direction;
            }
        }
    }
}
