using UnityEngine;

[System.Serializable]
public class CubicBezier
{
    public Vector3[] _Points;

    public CubicBezier()
    {
        _Points = new Vector3[4];
    }

    public Vector3 Evaluate(float t)
    {
        t = t % 1;
        if (t < 0)
            t++;

        return Mathf.Pow((1 - t), 3) * _Points[0] + 3 * Mathf.Pow((1 - t), 2) * t * _Points[1] + 3 * (1 - t) * Mathf.Pow(t, 2) * _Points[2] + Mathf.Pow(t, 3) * _Points[3];
    }
}
