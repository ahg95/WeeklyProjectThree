using System.Collections.Generic;
using UnityEngine;

public class EvenCubicBezierComposite : Path
{
    [SerializeField]
    CubicBezierComposite _bezierComposite;

    [SerializeField, Range(2, 100)]
    int _numberOfSamples;

    [SerializeField, HideInInspector]
    float[] _tAdjusted;

    [SerializeField, HideInInspector]
    AnimationCurve _tAdjustedCurve;

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

        return _bezierComposite.Evaluate(t);

        /*
        var lowerSampleIndex = Mathf.Clamp(Mathf.FloorToInt((_numberOfSamples - 1) * t), 0, _numberOfSamples - 2);
        var upperSampleIndex = lowerSampleIndex + 1;

        var lowerT = ((float)lowerSampleIndex) / (_numberOfSamples - 1);
        var upperT = ((float)upperSampleIndex) / (_numberOfSamples - 1);

        var alpha = Mathf.InverseLerp(lowerT, upperT, t);

        var lowerTAdjusted = _tAdjusted[lowerSampleIndex];
        var upperTAdjusted = _tAdjusted[upperSampleIndex];

        var adjustedT = Mathf.Lerp(lowerTAdjusted, upperTAdjusted, alpha);

        Debug.Log($"t: {t}, adjustedT: {adjustedT}, lowerIndex: {lowerSampleIndex}, upperIndex: {upperSampleIndex}");

        return _bezierComposite.Evaluate(adjustedT);
        */
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

            var sample = _bezierComposite.Evaluate(t);

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




        var totalLength = distanceToSamples[distanceToSamples.Count - 1];

        _tAdjustedCurve = new AnimationCurve();

        for (int i = 0; i < _numberOfSamples; i++)
        {
            var lengthProgress = distanceToSamples[i] / totalLength;

            var tAdjusted = ((float)i) / (_numberOfSamples - 1);

            _tAdjustedCurve.AddKey(lengthProgress, tAdjusted);
        }

        _isSetup = true;


        /*
        // Calculate the adjusted values for t
        // - Setup list
        _tAdjusted = new float[_numberOfSamples];

        var totalLength = distanceToSamples[distanceToSamples.Count - 1];

        int index = 0;

        // - Calculate adjusted t for all t values
        for (int i = 0; i < _numberOfSamples; i++)
        {
            //var t = 

            //while ()
            //{

            //}

            var tAdjusted = distanceToSamples[i] / totalLength;

            _tAdjusted[i] = tAdjusted;
        }

        _isSetup = true;
        */
    }
}
