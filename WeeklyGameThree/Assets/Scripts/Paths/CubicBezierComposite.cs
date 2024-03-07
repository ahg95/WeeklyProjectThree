using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CubicBezierComposite : Path
{
    public bool _IsCyclic;

    [SerializeField]
    int _numberOfCurves;

    public List<CubicBezier> _Curves;

    bool _isSetup;

    Vector3 _offset;
    float _angle;

    private void Awake()
    {
        _offset = transform.position;
    }

    private void Update()
    {
        if (!transform.hasChanged)
            return;


        SetupIfNecessary();



        // Round the position of this object
        transform.position = transform.position.Round();



        // Apply the rotation and position of this transform to all curve points
        foreach (var curve in _Curves)
        {
            for (int i = 0; i < curve._Points.Length; i++)
            {
                // Subtract the previous offset
                curve._Points[i] -= _offset;

                // Subtract the previous rotation
                curve._Points[i] = Quaternion.AngleAxis(-_angle, Vector3.forward) * curve._Points[i];

                // Add the new rotation
                curve._Points[i] = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * curve._Points[i];

                // Add the new offset
                curve._Points[i] += transform.position;
            }
        }

        // - Store the applied offset and rotation
        _offset = transform.position;
        _angle = transform.rotation.eulerAngles.z;
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
            _Curves.Add(new CubicBezier());
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
