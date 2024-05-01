using UnityEngine;

public class LinearPathRenderer : MonoBehaviour
{
    [SerializeField]
    LinearPath _linearPath;

    [SerializeField]
    LineRenderer[] _lineRenderers;

    void Start()
    {
        foreach (var renderer in _lineRenderers)
        {
            renderer.positionCount = _linearPath.NumberOfPoints;

            for (int i = 0; i < _linearPath.NumberOfPoints; i++)
            {
                var point = _linearPath.GetPoint(i);

                renderer.SetPosition(i, point);
            }

            renderer.loop = _linearPath.IsCyclic;
        }
    }
}
