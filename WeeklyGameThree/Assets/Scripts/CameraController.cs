using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Transform _target;

    [Header("Variables")]
    [SerializeField]
    Vector2Variable _minBounds;

    [SerializeField]
    Vector2Variable _maxBounds;

    [Header("Parameters")]
    [SerializeField]
    Vector2 _targetOffset;

    [SerializeField]
    [Range(0, 1)]
    float _smoothTime;

    [SerializeField]
    [Range(1, 100)]
    float _maxSpeed;

    float _cameraZ;
    Vector3 _smoothDampVelocity;
    bool _snapToTarget;


    private void Awake()
    {
        _cameraZ = _camera.transform.position.z;

        SnapToTarget();
    }

    void LateUpdate()
    {
        // Calculate the target position
        Vector3 targetPosition = _target.transform.position + Vector3.back * 10;

        // - Calculate the bounds of the viewport
        var viewportExtents = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);

        // - Calculate horizontal target position
        if (_maxBounds.RuntimeValue.x - _minBounds.RuntimeValue.x < 2 * viewportExtents.x)
        {
            targetPosition.x = (_minBounds.RuntimeValue.x + _maxBounds.RuntimeValue.x) / 2;
        } else
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, _minBounds.RuntimeValue.x + viewportExtents.x, _maxBounds.RuntimeValue.x - viewportExtents.x);
        }

        // - Calculate vertical target position
        if (_maxBounds.RuntimeValue.y - _minBounds.RuntimeValue.y < 2 * viewportExtents.y)
        {
            targetPosition.y = (_minBounds.RuntimeValue.y + _maxBounds.RuntimeValue.y) / 2;
        } else
        {
            targetPosition.y = Mathf.Clamp(targetPosition.y, _minBounds.RuntimeValue.y + viewportExtents.y, _maxBounds.RuntimeValue.y - viewportExtents.y);
        }



        // Move to the target position, either instantly or smoothly
        if (_snapToTarget)
        {
            transform.position = targetPosition;
            _snapToTarget = false;
        } else
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _smoothDampVelocity, _smoothTime, _maxSpeed, Time.deltaTime);
    }

    public void SnapToTarget()
    {
        _snapToTarget = true;
    }
}
