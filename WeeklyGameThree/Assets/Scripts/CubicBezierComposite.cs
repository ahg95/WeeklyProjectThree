using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CubicBezierComposite : MonoBehaviour
{
    public bool _IsCyclic;

    [SerializeField]
    int _numberOfCurves;

    public List<CubicBezier> _Curves;

    [SerializeField]
    List<float> _curveLengths;

    [SerializeField]
    float _compositeLength;

    [SerializeField]
    bool _lengthIsUpToDate;

    [SerializeField]
    bool _isSetup;

    Vector3 _offset;
    float _rotationAngle;

    

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
        transform.position = RoundVector(transform.position);



        // Apply the rotation and position of this transform to all curve points
        foreach (var curve in _Curves)
        {
            for (int i = 0; i < curve._Points.Length; i++)
            {
                // Subtract the previous offset
                curve._Points[i] -= _offset;

                // Subtract the previous rotation
                curve._Points[i] = Quaternion.AngleAxis(-_rotationAngle, Vector3.forward) * curve._Points[i];

                // Add the new rotation
                curve._Points[i] = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * curve._Points[i];

                // Add the new offset
                curve._Points[i] += transform.position;
            }
        }

        // - Store the applied offset and rotation
        _offset = transform.position;
        _rotationAngle = transform.rotation.eulerAngles.z;
    }

    public Vector3 Evaluate(float t)
    {
        SetupIfNecessary();

        Debug.Log(t);

        //if (t > 0.5f)
        //    Debug.Log(_compositeLength);


        if (_Curves.Count == 0)
            return transform.position;


        // Transform t to fit in [0,1)
        t = t % 1;
        if (t < 0)
            t++;


        // Calculate the index of the curve to evaluate
        int curveIndex = 0;

        float sumOfLengths = _curveLengths[0];

        t *= _compositeLength;

        while (sumOfLengths < t)
        {
            curveIndex++;
            sumOfLengths += _curveLengths[curveIndex];
        }



        // Transform t again to fit in [0,1)
        t = Mathf.InverseLerp(sumOfLengths - _curveLengths[curveIndex], sumOfLengths, t);

        for (int i = 0; i < _Curves.Count; i++)
        {
            sumOfLengths += _curveLengths[i];
        }


        // Evaluate the curve
        return _Curves[curveIndex].Evaluate(t);
    }


    private void OnValidate()
    {
        _isSetup = false;

        SetupIfNecessary();

        UpdateCurveLengthsIfNecessary();
    }

    void SetupIfNecessary()
    {
        if (_isSetup)
            return;

        Debug.Log("Setting up");

        if (_Curves == null)
            _Curves = new();

        _numberOfCurves = Mathf.Max(0, _numberOfCurves);

        while (_Curves.Count > _numberOfCurves)
        {
            _Curves.RemoveAt(_Curves.Count - 1);
            SetLengthAsDirty();
        }

        while (_Curves.Count < _numberOfCurves)
        {
            _Curves.Add(new CubicBezier());
            SetLengthAsDirty();
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
                curve._Points[i] = RoundVector(curve._Points[i]);

        EnforceConstraints();
    }

    Vector3 RoundVector(Vector3 vector)
    {
        const float multiplier = 2f;

        vector.x = Mathf.Round(vector.x * multiplier) / multiplier;
        vector.y = Mathf.Round(vector.y * multiplier) / multiplier;
        vector.z = Mathf.Round(vector.z * multiplier) / multiplier;

        return vector;
    }

    public void SetLengthAsDirty()
    {
        _lengthIsUpToDate = false;
    }

    void UpdateCurveLengthsIfNecessary()
    {
        if (_lengthIsUpToDate)
            return;



        // Setup list
        if (_curveLengths == null)
            _curveLengths = new();
        else
            _curveLengths.Clear();



        // Calculate lengths and complete length of composite
        _compositeLength = 0;

        for (int i = 0; i < _Curves.Count; i++)
        {
            var length = _Curves[i].CalculateLength(100);

            _curveLengths.Add(length);

            _compositeLength += length;
        }



        // Save that length is up to date
        _lengthIsUpToDate = true;
    }
}
