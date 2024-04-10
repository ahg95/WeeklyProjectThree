using UnityEngine;

public class DashIndicator : MonoBehaviour
{
    [SerializeField]
    BoolVariable _playerCanDash;

    [SerializeField]
    SpriteRenderer _dashIndicator;

    private void LateUpdate()
    {
        _dashIndicator.enabled = _playerCanDash.RuntimeValue;
    }
}
