using UnityEngine;

public class PlayerDashTrail : MonoBehaviour
{
    [SerializeField]
    TrailRenderer _trailRenderer;

    [SerializeField]
    BoolVariable _playerIsDashing;

    private void LateUpdate()
    {
        _trailRenderer.emitting = _playerIsDashing.RuntimeValue;
    }
}
