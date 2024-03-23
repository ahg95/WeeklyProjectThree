using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeTransformInCycles : MonoBehaviour, RoomObject
{
    // General
    [SerializeField]
    FloatVariable _timeScale;

    [SerializeField]
    float _baseOffset;

    // Movement
    [SerializeField]
    bool _moves;
    [SerializeField]
    bool _movesBackwards;
    [SerializeField]
    Path _path;
    [SerializeField]
    float _movementCycles = 1;
    [SerializeField]
    float _movementOffset;

    // Rotation
    [SerializeField]
    bool _rotates;
    [SerializeField]
    bool _rotatesCounterclockwise;
    [SerializeField]
    float _rotationCycles = 1;
    [SerializeField]
    float _rotationOffset;

    // Scaling
    [SerializeField]
    bool _scales;
    [SerializeField]
    float _scalingCycles = 1;
    [SerializeField]
    float _defaultScale;
    [SerializeField]
    float _changedScale;
    [SerializeField]
    float _scalingOffset;

    float _timer = 0;
    const float GLOBALCYCLETIME = 5f;

    private void Update()
    {
        _timer += Time.deltaTime * _timeScale.RuntimeValue;

        UpdatePositionIfMoves();

        UpdateRotationIfRotates();

        UpdateScaleIfScales();
    }

    private void UpdatePositionIfMoves()
    {
        if (_moves && _path != null)
        {
            var t = _timer / GLOBALCYCLETIME * _movementCycles + _baseOffset + _movementOffset;

            if (_movesBackwards)
                t = 1 - t;

            transform.position = _path.Evaluate(t);
        }
    }

    private void UpdateRotationIfRotates()
    {
        if (_rotates)
        {
            var angle = (_timer / GLOBALCYCLETIME * _rotationCycles + _baseOffset + _rotationOffset) * 360;

            angle = _rotatesCounterclockwise ? angle : -angle;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void UpdateScaleIfScales()
    {
        if (_scales)
        {
            var x = (_timer / GLOBALCYCLETIME * _scalingCycles + _baseOffset + _scalingOffset) * 2 * Mathf.PI;
            var y = Mathf.Cos(x) / 2 + 0.5f;
            var scale = Mathf.Lerp(_changedScale, _defaultScale, y);
            transform.localScale = Vector3.one * scale;
        }
    }

    private void OnValidate()
    {
        UpdatePositionIfMoves();
        UpdateRotationIfRotates();
        UpdateScaleIfScales();
    }

    public void ResetRoomObject()
    {
        _timer = 0;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeTransformInCycles)), CanEditMultipleObjects]
    public class MagicShapeEditor : Editor
    {
        // General
        SerializedProperty _timeScaleProperty;
        SerializedProperty _baseOffsetProperty;

        // Movement
        SerializedProperty _movesProperty;
        SerializedProperty _movesBackwardsProperty;
        SerializedProperty _pathProperty;
        SerializedProperty _movementCyclesProperty;
        SerializedProperty _movementOffsetProperty;

        // Rotation
        SerializedProperty _rotatesProperty;
        SerializedProperty _rotatesCounterclockwiseProperty;
        SerializedProperty _rotationCyclesProperty;
        SerializedProperty _rotationOffsetProperty;

        // Scaling
        SerializedProperty _scalesProperty;
        SerializedProperty _scalingCyclesProperty;
        SerializedProperty _defaultScaleProperty;
        SerializedProperty _changedScaleProperty;
        SerializedProperty _scalingOffsetProperty;

        private void OnEnable()
        {
            // General
            _timeScaleProperty = serializedObject.FindProperty("_timeScale");
            _baseOffsetProperty = serializedObject.FindProperty("_baseOffset");

            // Movement
            _movesProperty = serializedObject.FindProperty("_moves");
            _movesBackwardsProperty = serializedObject.FindProperty("_movesBackwards");
            _pathProperty = serializedObject.FindProperty("_path");
            _movementCyclesProperty = serializedObject.FindProperty("_movementCycles");
            _movementOffsetProperty = serializedObject.FindProperty("_movementOffset");

            // Rotation
            _rotatesProperty = serializedObject.FindProperty("_rotates");
            _rotatesCounterclockwiseProperty = serializedObject.FindProperty("_rotatesCounterclockwise");
            _rotationCyclesProperty = serializedObject.FindProperty("_rotationCycles");
            _rotationOffsetProperty = serializedObject.FindProperty("_rotationOffset");

            // Scaling
            _scalesProperty = serializedObject.FindProperty("_scales");
            _scalingCyclesProperty = serializedObject.FindProperty("_scalingCycles");
            _defaultScaleProperty = serializedObject.FindProperty("_defaultScale");
            _changedScaleProperty = serializedObject.FindProperty("_changedScale");
            _scalingOffsetProperty = serializedObject.FindProperty("_scalingOffset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // General
            EditorGUILayout.PropertyField(_timeScaleProperty);

            EditorGUILayout.Slider(_baseOffsetProperty, 0f, 1f);



            // Moves
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(_movesProperty);

            if (_movesProperty.boolValue)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(_movesBackwardsProperty);
                EditorGUILayout.PropertyField(_pathProperty);
                EditorGUILayout.Slider(_movementCyclesProperty, 0.125f, 10);
                EditorGUILayout.Slider(_movementOffsetProperty, 0f, 1f);

                EditorGUI.indentLevel = 0;
            }



            // Rotates
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(_rotatesProperty);

            if (_rotatesProperty.boolValue)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(_rotatesCounterclockwiseProperty);
                EditorGUILayout.Slider(_rotationCyclesProperty, 0.125f, 10);
                EditorGUILayout.Slider(_rotationOffsetProperty, 0f, 1f);

                EditorGUI.indentLevel = 0;
            }



            // Scales
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(_scalesProperty);

            if (_scalesProperty.boolValue)
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.Slider(_scalingCyclesProperty, 0.125f, 10);
                EditorGUILayout.Slider(_scalingOffsetProperty, 0f, 1f);
                EditorGUILayout.Slider(_defaultScaleProperty, 0f, 10f);
                EditorGUILayout.Slider(_changedScaleProperty, 0f, 10f);

                EditorGUI.indentLevel = 0;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
