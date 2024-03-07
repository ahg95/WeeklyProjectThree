using UnityEngine;

public abstract class Path : MonoBehaviour
{
    public abstract Vector3 Evaluate(float t);
}
