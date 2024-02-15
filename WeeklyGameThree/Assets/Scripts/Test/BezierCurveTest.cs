using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveTest : MonoBehaviour
{
    [SerializeField]
    Transform _toMove;

    [SerializeField]
    CubicBezierComposite _bezierComposite;

    [SerializeField]
    [Range(-2, 2)]
    float _t;

    private void OnValidate()
    {
        _toMove.transform.position = _bezierComposite.Evaluate(_t);
    }
}
