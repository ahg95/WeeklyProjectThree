using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubicBezierComposite))]
public class CubicBezierCompositeEditor : Editor
{
    private void OnSceneGUI()
    {
        // Visualize each curve and create handles for them

        var composite = (CubicBezierComposite)target;

        for (int i = 0; i < composite._Curves.Count; i++)
        {
            var curve = composite._Curves[i];

            var firstPoint = curve._Points[0];
            var secondPoint = curve._Points[1];


            // Create handle for the first point

            float size = HandleUtility.GetHandleSize(firstPoint) * 0.4f;
            Vector3 snap = Vector3.one * 0.5f;

            Handles.color = Color.blue;
            if (i == 0)
                Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.FreeMoveHandle(firstPoint, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                curve._Points[0] = newPosition;
                curve._Points[1] = newPosition + (secondPoint - firstPoint);

                // If this is the first curve, and the composite is cyclic, then adjust the points of the last curve as well.
                if (composite._IsCyclic && i == 0)
                {
                    var lastCurve = composite._Curves[composite._Curves.Count - 1];

                    lastCurve._Points[2] = newPosition + (lastCurve._Points[2] - lastCurve._Points[3]);
                    lastCurve._Points[3] = newPosition;
                } else if (i > 0)
                {
                    var previousCurve = composite._Curves[i - 1];

                    previousCurve._Points[2] = newPosition + (previousCurve._Points[2] - previousCurve._Points[3]);
                    previousCurve._Points[3] = newPosition;
                }

                composite.RoundCurvePointPositions();
                composite.SetLengthAsDirty();

                EditorUtility.SetDirty(composite);
            }



            // Create handle for second curve point
            size = HandleUtility.GetHandleSize(secondPoint) * 0.2f;
            snap = Vector3.one * 0.5f;

            Handles.color = Color.grey;
            Handles.DrawLine(firstPoint, secondPoint, 5);

            EditorGUI.BeginChangeCheck();

            Handles.color = Color.green;
            newPosition = Handles.FreeMoveHandle(secondPoint, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                curve._Points[1] = newPosition;

                if (composite._IsCyclic && i == 0)
                {
                    var lastCurve = composite._Curves[composite._Curves.Count - 1];

                    lastCurve._Points[2] = 2 * firstPoint - newPosition;
                } else if (i > 0)
                {
                    var previousCurve = composite._Curves[i - 1];

                    previousCurve._Points[2] = 2 * firstPoint - newPosition;
                }

                composite.RoundCurvePointPositions();
                composite.SetLengthAsDirty();

                EditorUtility.SetDirty(composite);
            }


            // If the composite is not cyclic and it is the last curve then draw the last point
            if (!composite._IsCyclic && i == composite._Curves.Count - 1)
            {
                var lastPoint = curve._Points[3];

                size = HandleUtility.GetHandleSize(lastPoint) * 0.4f;
                snap = Vector3.one * 0.5f;

                EditorGUI.BeginChangeCheck();

                Handles.color = Color.blue;
                newPosition = Handles.FreeMoveHandle(lastPoint, size, snap, Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    curve._Points[2] = newPosition + (curve._Points[2] - curve._Points[3]);
                    curve._Points[3] = newPosition;

                    composite.RoundCurvePointPositions();
                    composite.SetLengthAsDirty();

                    EditorUtility.SetDirty(composite);
                }
            }



            // Draw the curve
            Handles.color = Color.white;
            Handles.DrawBezier(curve._Points[0], curve._Points[3], curve._Points[1], curve._Points[2], Color.red, null, 5f);
        }
    }
}