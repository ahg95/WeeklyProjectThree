using UnityEngine;

public class CurvedPathRenderer : MonoBehaviour
{
    [SerializeField]
    LineRenderer[] _lineRenderer;

    [SerializeField]
    Path _path;

    [SerializeField, Range(10, 100)]
    int _numberOfSamples;

    void Start()
    {
        foreach (LineRenderer lineRenderer in _lineRenderer)
        {
            lineRenderer.positionCount = _numberOfSamples;

            for (int i = 0; i < _numberOfSamples; i++)
            {
                var t = (float)i / (_numberOfSamples - 1);

                if (i == _numberOfSamples - 1)
                    t = 0.9999f;

                var sample = _path.Evaluate(t);

                lineRenderer.SetPosition(i, sample);
            }
        }
    }
}
