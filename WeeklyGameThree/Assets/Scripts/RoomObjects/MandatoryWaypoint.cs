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

    bool _isSetup;

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
        _renderer.sprite = _reachedSprite;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;
    }

    public void ResetRoomObject()
    {
        SetupIfNecessary();

        if (_isReached)
            _nrOfReachedWaypoints--;

        _allWaypointsReached.RuntimeValue = _nrOfReachedWaypoints == _totalNrOfWaypoints;

        _isReached = false;
        _renderer.sprite = _unreachedSprite;
    }

    void SetupIfNecessary()
    {
        if (_isSetup)
            return;

        _playerLayer = LayerMask.NameToLayer("Player");
        _renderer = GetComponent<SpriteRenderer>();

        _isSetup = true;
    }
}
