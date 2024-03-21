using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DashCollectable : MonoBehaviour
{
    [SerializeField]
    BoolVariable _playerCanDash;

    [SerializeField]
    bool _RemoveWhenCollected;

    static int _playerLayer = -1;

    private void OnEnable()
    {
        if (_playerLayer == -1)
            _playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter");

        if (collision.gameObject.layer == _playerLayer && !_playerCanDash.RuntimeValue)
        {
            _playerCanDash.RuntimeValue = true;

            gameObject.SetActive(!_RemoveWhenCollected);
        }
    }
}
