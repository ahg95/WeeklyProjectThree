using UnityEngine;

public class MovingShootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    LinearPath _path;

    Shootable _shootable;

    bool _isMoving;

    int _targetPointIndex;
    Vector3 _targetPosition;
    bool _pointIndexIsDecreasing;

    const float SPEED = 2;

    private void Awake()
    {
        _shootable = GetComponent<Shootable>();
    }

    private void OnEnable()
    {
        _shootable._WasHit += OnShot;
    }

    private void OnDisable()
    {
        _shootable._WasHit -= OnShot;
    }

    private void OnValidate()
    {
        if (_path != null)
            transform.position = _path.GetPoint(0);
    }

    void OnShot(Vector2 hitDirection)
    {
        if (_isMoving)
            return;

        _isMoving = true;

        // Calculate target point index
        if (_pointIndexIsDecreasing)
        {
            _targetPointIndex--;

            if (_targetPointIndex < 0)
            {
                if (_path.IsCyclic)
                    _targetPointIndex += _path.NumberOfPoints;
                else
                {
                    _targetPointIndex += 2;
                    _pointIndexIsDecreasing = false;
                }
            }

        } else
        {
            _targetPointIndex++;

            if (_targetPointIndex > _path.NumberOfPoints - 1)
            {
                if (_path.IsCyclic)
                    _targetPointIndex -= _path.NumberOfPoints;
                else
                {
                    _targetPointIndex -= 2;
                    _pointIndexIsDecreasing = true;
                }
            }
        }


        // Set target position
        _targetPosition = _path.GetPoint(_targetPointIndex);
    }

    private void Update()
    {
        if (!_isMoving)
            return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * SPEED);

        if (transform.position != _targetPosition)
            return;

        _isMoving = false;
    }

    public void ResetRoomObject()
    {
        transform.position = _path.GetPoint(0);
        _targetPointIndex = 0;
        _targetPosition = _path.GetPoint(_targetPointIndex);
        _isMoving = false;
        _pointIndexIsDecreasing = false;
    }
}
