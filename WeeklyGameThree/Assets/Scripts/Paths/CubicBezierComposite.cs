using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class CubicBezierComposite : Path
{
    public bool _IsCyclic;

    [SerializeField]
    int _numberOfCurves;

    public List<CubicBezier> _Curves;

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
        // Visualize each curve and create handles for them

        var composite = this;

        for (int i = 0; i < composite._Curves.Count; i++)
        {
            var curve = composite._Curves[i];

            var firstPoint = curve._Points[0];
            var secondPoint = curve._Points[1];


            // Create handle for the first point

            float size = HandleUtility.GetHandleSize(firstPoint) * 0.4f;
            Vector3 snap = Vector3.one * 0.5f;

            Handles.color = Color.blue;
            if (i == 0)
                Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.FreeMoveHandle(firstPoint, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                curve._Points[0] = newPosition;
                curve._Points[1] = newPosition + (secondPoint - firstPoint);

                // If the composite is cyclic or this is not the first curve then there is a previous curve.
                // Some points of that curve must be adjusted as well.
                if (composite._IsCyclic || i > 0)
                {
                    var previousCurveIndex = composite._IsCyclic && i == 0 ? composite._Curves.Count - 1 : i - 1;

                    var previousCurve = composite._Curves[previousCurveIndex];

                    previousCurve._Points[2] = newPosition + (previousCurve._Points[2] - previousCurve._Points[3]);
                    previousCurve._Points[3] = newPosition;
                }

                EditorUtility.SetDirty(composite);
            }



            // Create handle for second curve point
            size = HandleUtility.GetHandleSize(secondPoint) * 0.2f;
            snap = Vector3.one * 0.5f;

            Handles.color = Color.grey;
            Handles.DrawLine(firstPoint, secondPoint, 5);

            EditorGUI.BeginChangeCheck();

            Handles.color = Color.green;
            newPosition = Handles.FreeMoveHandle(secondPoint, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                curve._Points[1] = newPosition;

                // If the composite is cyclic or this is not the first curve then there is a previous curve.
                // Some points of that curve must be adjusted as well.
                if (composite._IsCyclic || i > 0)
                {
                    var previousCurveIndex = composite._IsCyclic && i == 0 ? composite._Curves.Count - 1 : i - 1;

                    var previousCurve = composite._Curves[previousCurveIndex];

                    previousCurve._Points[2] = 2 * firstPoint - newPosition;
                }

                EditorUtility.SetDirty(composite);
            }


            // If the composite is not cyclic and it is the last curve then draw the last two points of the curve
            if (!composite._IsCyclic && i == composite._Curves.Count - 1)
            {
                // Create handle for the last point
                var lastPoint = curve._Points[3];

                size = HandleUtility.GetHandleSize(lastPoint) * 0.4f;
                snap = Vector3.one * 0.5f;

                EditorGUI.BeginChangeCheck();

                Handles.color = Color.blue;
                newPosition = Handles.FreeMoveHandle(lastPoint, size, snap, Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    curve._Points[2] = newPosition + (curve._Points[2] - curve._Points[3]);
                    curve._Points[3] = newPosition;

                    EditorUtility.SetDirty(composite);
                }



                // Create handle for the second last point
                var secondLastPoint = curve._Points[2];

                size = HandleUtility.GetHandleSize(secondLastPoint) * 0.2f;
                snap = Vector3.one * 0.5f;

                Handles.color = Color.grey;
                Handles.DrawLine(secondLastPoint, lastPoint, 5);

                EditorGUI.BeginChangeCheck();

                Handles.color = Color.green;
                newPosition = Handles.FreeMoveHandle(secondLastPoint, size, snap, Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    curve._Points[2] = newPosition;

                    EditorUtility.SetDirty(composite);
                }

            }



            // Draw the curve
            Handles.color = Color.white;
            Handles.DrawBezier(curve._Points[0], curve._Points[3], curve._Points[1], curve._Points[2], Color.red, null, 5f);
        }
    }

#endif

    private void Awake()
    {
        _offset = transform.position;
        _angle = transform.rotation.eulerAngles.z;
        _scale = transform.localScale;
    }


    private void Update()
    {
        if (!transform.hasChanged)
            return;


        SetupIfNecessary();



        // Apply the rotation, scale, and position of this transform to all curve points
        foreach (var curve in _Curves)
        {
            for (int i = 0; i < curve._Points.Length; i++)
            {
                // Subtract the previous offset
                curve._Points[i] -= _offset;

                // Subtract the previous rotation
                curve._Points[i] = Quaternion.AngleAxis(-_angle, Vector3.forward) * curve._Points[i];

                // Subtract the scale
                curve._Points[i].x = curve._Points[i].x * (1.0f /_scale.x);
                curve._Points[i].y = curve._Points[i].y * (1.0f / _scale.y);
                curve._Points[i].z = curve._Points[i].z * (1.0f / _scale.z);

                // Add the new scale
                curve._Points[i].x = curve._Points[i].x * transform.localScale.x;
                curve._Points[i].y = curve._Points[i].y * transform.localScale.y;
                curve._Points[i].z = curve._Points[i].z * transform.localScale.z;

                // Add the new rotation
                curve._Points[i] = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * curve._Points[i];

                // Add the new offset
                curve._Points[i] += transform.position;
            }
        }

        // - Store the applied offset and rotation
        _offset = transform.position;
        _angle = transform.rotation.eulerAngles.z;
        _scale = transform.localScale;
    }

    public override Vector3 Evaluate(float t)
    {
        SetupIfNecessary();

        if (_Curves.Count == 0)
            return transform.position;

        t = t % 1;
        if (t < 0)
            t++;

        var curveSpan = 1.0f / _Curves.Count;
        var curveIndex = Mathf.FloorToInt(t / curveSpan) % _Curves.Count;
        t = Mathf.InverseLerp(curveIndex * curveSpan, (curveIndex + 1) * curveSpan, t);
        return _Curves[curveIndex].Evaluate(t);
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

        if (_Curves == null)
            _Curves = new();

        _numberOfCurves = Mathf.Max(0, _numberOfCurves);

        while (_Curves.Count > _numberOfCurves)
        {
            _Curves.RemoveAt(_Curves.Count - 1);
        }

        while (_Curves.Count < _numberOfCurves)
        {
            var cubicBezier = new CubicBezier();

            var creationPosition = _Curves.Count == 0 ? transform.position : _Curves.Last()._Points[3];

            cubicBezier._Points[0] = creationPosition;
            cubicBezier._Points[1] = creationPosition + new Vector3(1, 1, 0);
            cubicBezier._Points[2] = creationPosition + new Vector3(2, 2, 0);
            cubicBezier._Points[3] = creationPosition + new Vector3(3, 3, 0);

            _Curves.Add(cubicBezier);
        }

        EnforceConstraints();

        _isSetup = true;
    }

    public void EnforceConstraints()
    {
        for (int i = 0; i < _Curves.Count; i++)
        {
            var curve = _Curves[i];

            if (i < _Curves.Count - 1 || _IsCyclic)
            {
                var nextCurve = _Curves[(i + 1) % _Curves.Count];

                curve._Points[3] = nextCurve._Points[0];
                curve._Points[2] = 2 * curve._Points[3] - nextCurve._Points[1];
            }
        }
    }

    public void RoundCurvePointPositions()
    {
        foreach (var curve in _Curves)
            for (int i = 0; i < curve._Points.Length; i++)
                curve._Points[i] = curve._Points[i].Round();

        EnforceConstraints();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CubicBezierComposite))]
public class CubicBezierCompositeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Snap curve points"))
        {
            var composite = (CubicBezierComposite)target;

            composite.RoundCurvePointPositions();
        }
    }
}
#endif