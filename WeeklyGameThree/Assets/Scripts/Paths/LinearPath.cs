using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LinearPath : Path
{
    [SerializeField]
    bool _isCyclic;

    public bool IsCyclic
    {
        get { return _isCyclic; }
    }

    [SerializeField]
    int _numberOfPoints;

    public int NumberOfPoints
    {
        get { return _numberOfPoints; }
    }

    [SerializeField, HideInInspector]
    List<Vector3> _points;

    public Vector3 GetPoint(int index)
    {
        index = index % _points.Count;
        index += index < 0 ? _points.Count : 0;
        return _points[index];
    }

    [SerializeField, HideInInspector]
    List<float> _distances;

    [SerializeField, HideInInspector]
    float _totalDistance;

    [SerializeField, HideInInspector]
    bool _isSetup;

    [SerializeField, HideInInspector]
    Vector3 _offset;

    [SerializeField, HideInInspector]
    float _angle;

    [SerializeField, HideInInspector]
    Vector3 _scale = Vector3.one;

#if UNITY_EDITOR

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        var path = this;

        // Visualize each point

        for (int i = 0; i < path._points.Count; i++)
        {
            var pointPosition = path._points[i];


            // Create handle for the first point
            float size = HandleUtility.GetHandleSize(pointPosition) * 0.4f;
            Vector3 snap = Vector3.one * 0.5f;

            Handles.color = Color.blue;
            if (i == 0)
                Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.FreeMoveHandle(pointPosition, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                path._points[i] = newPosition;
                EditorUtility.SetDirty(path);
            }


            if (!path._isCyclic && i == path._points.Count - 1)
                continue;

            // Draw the line
            Handles.color = Color.white;
            Handles.DrawLine(pointPosition, path._points[(i + 1) % path._points.Count], 5f);
        }
    }
#endif


    void Update()
    {
        if (!transform.hasChanged)
            return;


        SetupIfNecessary();


        // Apply the rotation and position of this transform to all curve points
        for (int i = 0; i < _points.Count; i++)
        {
            // Subtract the previous offset
            _points[i] -= _offset;

            // Subtract the previous rotation
            _points[i] = Quaternion.AngleAxis(-_angle, Vector3.forward) * _points[i];

            // Subtract the previous scale
            var point = _points[i];

            point.x = point.x * (1.0f / _scale.x);
            point.y = point.y * (1.0f / _scale.y);
            point.z = point.z * (1.0f / _scale.z);

            // Add the new scale

            point.x = point.x * transform.localScale.x;
            point.y = point.y * transform.localScale.y;
            point.z = point.z * transform.localScale.z;

            _points[i] = point;

            // Add the new rotation
            _points[i] = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * _points[i];

            // Add the new offset
            _points[i] += transform.position;
        }

        // Store the applied offset and rotation
        _offset = transform.position;
        _angle = transform.rotation.eulerAngles.z;
        _scale = transform.localScale;



        CalculateDistances();
    }

    public override Vector3 Evaluate(float t)
    {
        SetupIfNecessary();


        if (_points.Count == 0)
            return transform.position;


        // Transform t into the range [0, 1)
        t = t % 1;
        if (t < 0)
            t++;



        var targetDistance = t * _totalDistance;

        int segmentStartPointIndex = 0;
        var segmentMinDistance = 0f;
        var segmentMaxDistance = _distances[0];

        while (segmentMaxDistance < targetDistance) {

            segmentStartPointIndex++;

            segmentMinDistance = segmentMaxDistance;
            segmentMaxDistance += _distances[segmentStartPointIndex];
        }

        t = Mathf.InverseLerp(segmentMinDistance, segmentMaxDistance, targetDistance);

        return Vector3.Lerp(_points[segmentStartPointIndex], _points[(segmentStartPointIndex + 1) % _points.Count], t);
    }

    private void OnValidate()
    {
        _isSetup = false;

        SetupIfNecessary();
    }

    void SetupIfNecessary()
    {
        if (_isSetup)
            return;


        // Setup points
        _numberOfPoints = Mathf.Max(0, _numberOfPoints);

        if (_points == null)
            _points = new(_numberOfPoints);

        while (_points.Count > _numberOfPoints)
        {
            _points.RemoveAt(_points.Count - 1);
        }

        while (_points.Count < _numberOfPoints)
        {
            var lastPoint = _points[_points.Count - 1];

            _points.Add(lastPoint + new Vector3(1, 1, 0));
        }


        CalculateDistances();
    }

    public void CalculateDistances()
    {
        if (_distances == null)
            _distances = new(_numberOfPoints);
        else
            _distances.Clear();

        _totalDistance = 0;

        for (int i = 0; i < _numberOfPoints; i++)
        {
            var distance = Vector3.Distance(_points[i], _points[(i + 1) % _numberOfPoints]);
            _distances.Add(distance);

            if (_isCyclic || i != _numberOfPoints - 1)
                _totalDistance += distance;
        }
    }

    public void RoundPointPositions()
    {
        for (int i = 0; i < _points.Count; i++)
            _points[i] = _points[i].Round();
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(LinearPath)), CanEditMultipleObjects]
    public class LinearPathEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Snap curve points"))
            {
                var composite = (LinearPath)target;

                composite.RoundPointPositions();

                EditorUtility.SetDirty(composite);
            }
        }
    }
#endif