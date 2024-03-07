using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Round(this Vector3 vector, float multiplier = 2f)
    {
        vector.x = Mathf.Round(vector.x * multiplier) / multiplier;
        vector.y = Mathf.Round(vector.y * multiplier) / multiplier;
        vector.z = Mathf.Round(vector.z * multiplier) / multiplier;

        return vector;
    }
}
