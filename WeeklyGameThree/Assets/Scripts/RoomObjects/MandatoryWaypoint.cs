using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MandatoryWaypoint : MonoBehaviour, RoomObject
{
    static int _totalNrOfWaypoints;

    static int _nrOfReachedWaypoints;

    static int _playerLayer = -1;

    [Header("References")]
    [SerializeField]
    Animator _animator;

    [Header("Game Variables")]
    [SerializeField]
    BoolVariable _allWaypointsReached;

    bool _isReached;

    private void Awake()
    {
        if (_playerLayer == -1)
            _playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnEnable()
    {
        _totalNrOfWaypoints++;

        ResetRoomObject();
    }

    private void OnDisable()
    {
        _totalNrOfWaypoints--;

        ResetRoomObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isReached || collision.gameObject.layer != _playerLayer)
            return;

        // The mandatory waypoint has been reached
        _nrOfReachedWaypoints++;
        _isReached = true;


        // Trigger the appropriate animation
        _animator.SetBool("Reached", true);


        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;
    }

    public void ResetRoomObject()
    {
        if (_isReached)
            _nrOfReachedWaypoints--;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;

        _isReached = false;

        _animator.SetBool("Reached", false);
    }
}
