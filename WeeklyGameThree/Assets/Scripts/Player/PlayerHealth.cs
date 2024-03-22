using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    BoolVariable _playerIsInsideSafeZone;

    [SerializeField]
    FloatVariable _currentHealth;

    [SerializeField]
    SimpleGameEvent _healthDepleted;

    [SerializeField]
    RuntimeSet<Collider2D> _magicShapes;

    [Header("Parameters")]
    [SerializeField]
    float _damagePerSecond;

    [SerializeField]
    Vector2 _sampleAreaSize;

    [SerializeField]
    float _samplingDistance;

    List<Vector2> _samplePositions = new();

    float _maxHealth;

    private void Awake()
    {
        _maxHealth = _currentHealth.RuntimeValue;
    }

    private void OnValidate()
    {
        // Calculate sample positions
        _samplePositions.Clear();

        var nrOfHorizontalPoints = Mathf.Clamp(Mathf.FloorToInt(_sampleAreaSize.x / _samplingDistance), 2, 10);
        var nrOfVerticalPoints = Mathf.Clamp(Mathf.FloorToInt(_sampleAreaSize.y / _samplingDistance), 2, 10);

        var horizontalDistance = _sampleAreaSize.x / (nrOfHorizontalPoints - 1);
        var verticalDistance = _sampleAreaSize.y / (nrOfVerticalPoints - 1);

        for (int widthIndex = 0; widthIndex < nrOfHorizontalPoints; widthIndex++)
        {
            for (int heightIndex = 0; heightIndex < nrOfVerticalPoints; heightIndex++)
            {
                Vector2 point = new Vector2(widthIndex * horizontalDistance, heightIndex * verticalDistance);
                point -= _sampleAreaSize / 2;

                _samplePositions.Add(point);
            }
        }
    }

    void Update()
    {
        AdjustHealth();
    }

    void AdjustHealth()
    {
        // No need to reduce the health if it is already 0 or if the player is inside a safe zone
        if (_currentHealth.RuntimeValue == 0 || _playerIsInsideSafeZone.RuntimeValue)
            return;



        // Check how many sampling points are inside magic
        var nrOfSamplingPointsInMagic = 0;

        foreach (var point in _samplePositions)
        {
            var numberOfOverlappingShapes = 0;

            // Iterate through all magic shapes
            for (int i = 0; i < _magicShapes.Count; i++)
            {
                var magicShape = _magicShapes.Get(i);

                if (magicShape.gameObject.activeInHierarchy && magicShape.OverlapPoint((Vector2)transform.position + point))
                    numberOfOverlappingShapes++;
            }

            nrOfSamplingPointsInMagic += numberOfOverlappingShapes % 2;
        }



        // Determine damage based on the number of points inside magic
        if (nrOfSamplingPointsInMagic == 0)
        {
            _currentHealth.RuntimeValue = Mathf.Min(_maxHealth, _currentHealth.RuntimeValue + 250 * Time.deltaTime);
        } else if (nrOfSamplingPointsInMagic == _samplePositions.Count)
        {
            _currentHealth.RuntimeValue = Mathf.Max(0, _currentHealth.RuntimeValue - 750 * Time.deltaTime);
        } else
        {
            _currentHealth.RuntimeValue = Mathf.Max(0, _currentHealth.RuntimeValue - 250 * ((float)nrOfSamplingPointsInMagic / _samplePositions.Count) * Time.deltaTime);
        }

        if (_currentHealth.RuntimeValue == 0)
            _healthDepleted.Raise();
    }

    public void ResetHealth()
    {
        _currentHealth.RuntimeValue = _maxHealth;
    }

#if UNITY_EDITOR
    public class PlayerHealthEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected)]
        static void DrawGizmo(PlayerHealth playerHealth, GizmoType gizmo)
        {
            Gizmos.color = Color.blue;
            // Draw the box
            Gizmos.DrawWireCube(playerHealth.transform.position, new Vector3(playerHealth._sampleAreaSize.x, playerHealth._sampleAreaSize.y));



            // Draw each sampling point
            foreach (var point in playerHealth._samplePositions)
            {
                Gizmos.DrawSphere(playerHealth.transform.position + (Vector3)point, 0.1f);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        }
    }
#endif
}