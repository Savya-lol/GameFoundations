using UnityEngine;

namespace Savya.GameFoundations.GameLoop.Benchmark
{
    public class ManagedUpdateBenchmark : MonoBehaviour, IUpdatable
    {
        [SerializeField] private float _rotationSpeed = 45f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private bool _doCalculations = true;
        [SerializeField] private int _priority = 0;

        private Vector3 _direction;
        private float _calculation;

        private void Start()
        {
            _direction = Random.insideUnitSphere;
            UpdateManager.Instance?.RegisterUpdatable(this, _priority);
        }

        private void OnDestroy()
        {
            UpdateManager.Instance?.UnregisterUpdatable(this);
        }

        public void ManagedUpdate()
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
