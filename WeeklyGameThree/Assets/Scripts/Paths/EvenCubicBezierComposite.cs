using System.Collections.Generic;
using UnityEngine;

public class EvenCubicBezierComposite : Path
{
    public CubicBezierComposite _BezierComposite;

    [SerializeField, Range(2, 100)]
    int _numberOfSamples;

    [SerializeField, HideInInspector]
    float[] _tAdjusted;

    [SerializeField, HideInInspector]
    AnimationCurve _tAdjustedCurve;

    public float TotalLength
    {
        get {
            SetupIfNecessary();

            return _totalLength;
        }
    }

    float _totalLength;

    bool _isSetup;

    private void Awake()
    {
        SetupIfNecessary();
    }

    public override Vector3 Evaluate(float t)
    {
        SetupIfNecessary();

        t = t % 1;
        if (t < 0)
            t++;

        t = _tAdjustedCurve.Evaluate(t);

        return _BezierComposite.Evaluate(t);
    }

    void SetupIfNecessary()
    {
        if (_isSetup)
            return;

        // Sample bezier composite
        // - Setup list
        var distanceToSamples = new List<float>(_numberOfSamples);

        Vector3 previousSample = Vector3.zero;

        // - Calculate distances to samples
        for (int i = 0; i < _numberOfSamples; i++)
        {
            var t = (float)i / (_numberOfSamples - 1);

            // t should never be equal to 1 or it will not calculate paths correctly that are not cyclic
            if (i == _numberOfSamples - 1)
                t = 0.9999f;

            var sample = _BezierComposite.Evaluate(t);

            if (i == 0)
            {
                distanceToSamples.Add(0);
            }
            else
            {
                var distance = distanceToSamples[i - 1] + Vector3.Distance(previousSample, sample);
                distanceToSamples.Add(distance);
            }

            previousSample = sample;
        }



        _totalLength = distanceToSamples[distanceToSamples.Count - 1];

        _tAdjustedCurve = new AnimationCurve();

        for (int i = 0; i < _numberOfSamples; i++)
        {
            var lengthProgress = distanceToSamples[i] / _totalLength;

            var tAdjusted = ((float)i) / (_numberOfSamples - 1);

            _tAdjustedCurve.AddKey(lengthProgress, tAdjusted);
        }

        _isSetup = true;
    }
}
