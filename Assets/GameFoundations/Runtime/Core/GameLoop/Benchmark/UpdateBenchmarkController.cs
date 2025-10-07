using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Savya.GameFoundations.GameLoop.Benchmark
{
    public class UpdateBenchmarkController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _normalUpdatePrefab;
        [SerializeField] private GameObject _managedUpdatePrefab;

        [Header("Settings")]
        [SerializeField] private int _objectCount = 1000;
        
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private Key _toggleKey = Key.Space;
#else
        [SerializeField] private KeyCode _toggleKey = KeyCode.Space;
#endif

        [Header("UI")]
        [SerializeField] private TMP_Text _statsText;

        private GameObject[] _normalObjects;
        private GameObject[] _managedObjects;
        private bool _useNormalUpdate = true;
        private Stopwatch _stopwatch = new Stopwatch();
        private float _averageFrameTime;
        private const int SAMPLE_SIZE = 60;
        private float[] _frameSamples = new float[SAMPLE_SIZE];
        private int _sampleIndex = 0;

        private void Start()
        {
            SpawnObjects();
        }

        private void Update()
        {
            if (CheckToggleInput())
            {
                ToggleUpdateMode();
            }

            MeasurePerformance();
            UpdateUI();
        }

        private bool CheckToggleInput()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current[_toggleKey].wasPressedThisFrame;
#else
            return Input.GetKeyDown(_toggleKey);
#endif
        }

        private void SpawnObjects()
        {
            // Clear existing
            ClearObjects();

            if (_useNormalUpdate && _normalUpdatePrefab != null)
            {
                _normalObjects = new GameObject[_objectCount];
                for (int i = 0; i < _objectCount; i++)
                {
                    Vector3 pos = new Vector3(
                        Random.Range(-50f, 50f),
                        Random.Range(-50f, 50f),
                        Random.Range(-50f, 50f)
                    );
                    _normalObjects[i] = Instantiate(_normalUpdatePrefab, pos, Quaternion.identity);
                }
            }
            else if (!_useNormalUpdate && _managedUpdatePrefab != null)
            {
                _managedObjects = new GameObject[_objectCount];
                for (int i = 0; i < _objectCount; i++)
                {
                    Vector3 pos = new Vector3(
                        Random.Range(-50f, 50f),
                        Random.Range(-50f, 50f),
                        Random.Range(-50f, 50f)
                    );
                    _managedObjects[i] = Instantiate(_managedUpdatePrefab, pos, Quaternion.identity);
                }
            }
        }

        private void ClearObjects()
        {
            if (_normalObjects != null)
            {
                foreach (var obj in _normalObjects)
                {
                    if (obj != null) Destroy(obj);
                }
                _normalObjects = null;
            }

            if (_managedObjects != null)
            {
                foreach (var obj in _managedObjects)
                {
                    if (obj != null) Destroy(obj);
                }
                _managedObjects = null;
            }
        }

        private void ToggleUpdateMode()
        {
            _useNormalUpdate = !_useNormalUpdate;
            SpawnObjects();
            _sampleIndex = 0;
            _averageFrameTime = 0;
        }

        private void MeasurePerformance()
        {
            _frameSamples[_sampleIndex] = Time.deltaTime * 1000f; // Convert to ms
            _sampleIndex = (_sampleIndex + 1) % SAMPLE_SIZE;

            float total = 0;
            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                total += _frameSamples[i];
            }
            _averageFrameTime = total / SAMPLE_SIZE;
        }

        private void UpdateUI()
        {
            if (_statsText == null) return;

            string mode = _useNormalUpdate ? "Normal Update" : "UpdateManager";
            int fps = Mathf.RoundToInt(1000f / _averageFrameTime);
            
            _statsText.text = $"Mode: {mode}\n" +
                             $"Objects: {_objectCount}\n" +
                             $"FPS: {fps}\n" +
                             $"Frame Time: {_averageFrameTime:F2}ms\n" +
                             $"Press {GetToggleKeyName()} to toggle";
        }

        private string GetToggleKeyName()
        {
#if ENABLE_INPUT_SYSTEM
            return _toggleKey.ToString();
#else
            return _toggleKey.ToString();
#endif
        }

        private void OnDestroy()
        {
            ClearObjects();
        }
    }
}
