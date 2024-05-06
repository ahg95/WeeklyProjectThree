using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class CircularPath : Path
{
    public override Vector3 Evaluate(float t)
    {
        Vector3 result = transform.position;

        result += Mathf.Sin(t * 2 * Mathf.PI) * Vector3.up * transform.localScale.y;
        result += Mathf.Cos(t * 2 * Mathf.PI) * Vector3.left * transform.localScale.x;

        return result;
    }

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
        Handles.color = Color.blue;

        var xScale = transform.localScale.x;
        var yScale = transform.localScale.y;

        int numberOfPoints =  8 + Mathf.FloorToInt((xScale + yScale) / 2) * 8;

        List<Vector3> points = new List<Vector3>(numberOfPoints);

        for (int i = 0; i < numberOfPoints; i++)
        {
            var x = transform.position.x + Mathf.Cos(((float)i) / (numberOfPoints - 1) * 2 * Mathf.PI) * xScale;
            var y = transform.position.y + Mathf.Sin(((float)i) / (numberOfPoints - 1) * 2 * Mathf.PI) * yScale;

            points.Add(new Vector3(x, y, 0));
        }

        Handles.DrawPolyLine(points.ToArray());
    }
#endif
}
