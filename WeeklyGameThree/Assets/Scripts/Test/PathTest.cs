using UnityEngine;

public class PathTest : MonoBehaviour
{
    [SerializeField]
    Path _path;

    [SerializeField]
    Transform _follower;

    private void Update()
    {
        _follower.transform.position = _path.Evaluate(Time.time);
    }
}
