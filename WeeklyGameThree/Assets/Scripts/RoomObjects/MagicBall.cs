using System.Collections.Generic;
using UnityEngine;

public class MagicBall : MonoBehaviour, RoomObject
{
    [SerializeField]
    Transform _waypointsContainer;

    [SerializeField]
    EvenCubicBezierComposite[] _paths;

    [SerializeField]
    FloatVariable _timeScale;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    MagicBallWaypoint _waypointPrefab;

    [SerializeField]
    float _startStopDelay;

    List<MagicBallWaypoint> _waypoints;

    bool _isCyclic;

    int _pathIndex = 0;

    bool _isMovingBackward;

    float _startProgressTime;

    float _stopMoveTime;

    bool _isMoving;

    static int _playerLayer = -1;

    const float speed = 3;

    private void Awake()
    {
        // Check if the paths form a cycle or not
        var firstPath = _paths[0];
        var startPoint = firstPath._BezierComposite.Points[0];

        var lastPath = _paths[_paths.Length - 1];
        var lastPathPoints = lastPath._BezierComposite.Points;
        var endPoint = lastPathPoints[lastPathPoints.Count - 1];

        _isCyclic = startPoint == endPoint;



        // Instantiate waypoints
        // - There is a waypoint at every start of each path

        _waypoints = new (_paths.Length + (_isCyclic ? 1 : 0));

        for (int i = 0; i < _paths.Length; i++)
        {
            var path = _paths[i];

            var firstPathPoint = path._BezierComposite.Points[0];

            var waypoint = Instantiate(_waypointPrefab, firstPathPoint, Quaternion.identity, _waypointsContainer);

            _waypoints.Add(waypoint);
        }

        // - There is also a waypoint at the end of the last path if the paths form a cycle
        if (!_isCyclic)
        {
            var waypoint = Instantiate(_waypointPrefab, endPoint, Quaternion.identity, _waypointsContainer);

            _waypoints.Add(waypoint);
        }



        ResetRoomObject();



        // Establish player layer
        if (_playerLayer == -1)
            _playerLayer = LayerMask.NameToLayer("Player");
    }

    public void Progress()
    {
        // Play activation animation of magic ball
        _animator.SetTrigger("Activate");



        // Show the current waypoint as inactive
        int currentWaypointIndex;

        if (!_isMovingBackward)
            currentWaypointIndex = (_pathIndex + 1) % _waypoints.Count;
        else
            currentWaypointIndex = _pathIndex;

        var currentWaypoint = _waypoints[currentWaypointIndex];

        currentWaypoint.ShowAsInactive();



        // Calculate the next path index
        if (_isCyclic)
        {
            _pathIndex++;
            _pathIndex %= _paths.Length;
        } else if (_isMovingBackward)
        {
            _pathIndex--;

            if (_pathIndex < 0)
            {
                _pathIndex = 0;
                _isMovingBackward = false;
            }

        } else
        {
            _pathIndex++;

            if (_pathIndex == _paths.Length)
            {
                _pathIndex = _paths.Length - 1;
                _isMovingBackward = true;
            }
        }



        // Start moving the magic ball
        _isMoving = true;
        _startProgressTime = Time.time;
    }

    private void Update()
    {
        // Only update the position if this thing is moving
        if (!_isMoving)
            return;


        var path = _paths[_pathIndex];


        // Calculate progress of movement
        var progress = Mathf.Clamp01((Time.time - _startProgressTime - _startStopDelay) * speed / path.TotalLength);



        // If the magic ball reached the destination then stop it and play necessary animations
        if (progress == 1)
        {
            // - Play ball deactivation animation
            _animator.SetTrigger("Deactivate");



            // - Play waypoint activation animation
            int currentWaypointIndex;

            if (!_isMovingBackward)
                currentWaypointIndex = (_pathIndex + 1) % _waypoints.Count;
            else
                currentWaypointIndex = _pathIndex;

            var currentWaypoint = _waypoints[currentWaypointIndex];

            currentWaypoint.ShowAsActive();



            // - Stop the ball
            _isMoving = false;
            _stopMoveTime = Time.time;
        }



        // Update position of magic ball
        // - Calculate t value for bezier curve
        const float BELOWONE = 0.99999f;

        float t;

        if (_isMovingBackward)
            t = Mathf.Lerp(BELOWONE, 0, progress);
        else
            t = Mathf.Lerp(0, BELOWONE, progress);

        // - Set new position
        var nextPosition = path.Evaluate(t);

        transform.position = nextPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isMoving || collision.gameObject.layer != _playerLayer || Time.time < _stopMoveTime + _startStopDelay)
            return;

        Progress();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isMoving || collision.gameObject.layer != _playerLayer || Time.time < _stopMoveTime + _startStopDelay)
            return;

        Progress();
    }

    public void ResetRoomObject()
    {
        // Stop any previous movement
        _isMoving = false;



        // Initialize the path index such that it traverses the first path first
        if (_isCyclic)
            _pathIndex = _paths.Length - 1;
        else
            _isMovingBackward = true;



        // Show the first waypoint as being the active one
        for (int i = 0; i < _waypoints.Count; i++)
        {
            var waypoint = _waypoints[i];

            if (i == 0)
                waypoint.ShowAsActiveImmediately();
            else
                waypoint.ShowAsInactiveImmediately();
        }



        // Initialize magic ball
        transform.position = _waypoints[0].transform.position;
        _animator.SetTrigger("DeactivateImmediately");
    }
}
