using UnityEngine;

public class DashIndicator : MonoBehaviour
{
    [SerializeField]
    BoolVariable _playerCanDash;

    [SerializeField]
    GameObject _dashIndicator;

    private void LateUpdate()
    {
        _dashIndicator.SetActive(_playerCanDash.RuntimeValue);
    }
}
