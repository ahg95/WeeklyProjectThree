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
                var sample = _path.Evaluate((float)i / (_numberOfSamples - 1));

                lineRenderer.SetPosition(i, sample);
            }
        }
    }
}
