using UnityEngine;
using Unity.VisualScripting;

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
        result += Mathf.Cos(t * 2 * Mathf.PI) * Vector3.right * transform.localScale.x;

        return result;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CircularPath)), CanEditMultipleObjects]
    public class CircularPathEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected)]
        static void DrawGizmo(CircularPath circularPath, GizmoType gizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(circularPath.transform.position, circularPath.transform.localScale.x);
        }
    }
#endif
}
