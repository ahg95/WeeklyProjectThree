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
    Sprite _unreachedSprite;

    [SerializeField]
    Sprite _reachedSprite;

    [Header("Game Variables")]
    [SerializeField]
    BoolVariable _allWaypointsReached;

    SpriteRenderer _renderer;

    bool _isReached;

    private void Awake()
    {
        _playerLayer = LayerMask.NameToLayer("Player");
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _totalNrOfWaypoints++;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;
    }

    private void OnDisable()
    {
        _totalNrOfWaypoints--;

        if (_isReached)
            _nrOfReachedWaypoints--;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isReached || collision.gameObject.layer != _playerLayer)
            return;

        // The mandatory waypoint has been reached
        _nrOfReachedWaypoints++;
        _isReached = true;
        _renderer.sprite = _reachedSprite;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;
    }

    public void ResetRoomObject()
    {
        if (_isReached)
            _nrOfReachedWaypoints--;

        _isReached = false;
        _renderer.sprite = _unreachedSprite;
    }
}
