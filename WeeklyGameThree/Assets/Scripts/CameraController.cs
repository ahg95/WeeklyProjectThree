using System.Collections;
using System.Collections.Generic;
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
        // Track the target
        // - Calculate the target position
        Vector3 targetPosition = _target.transform.position + Vector3.forward * _cameraZ + (Vector3)_targetOffset;



        // - Adjust the target position such that the camera viewport will stay within the room bounds,
        // or such that it centers the room staying within the bounds of the room is impossible
        var halfViewport = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);

        // - - Adjust vertical position
        var topOverlap = Mathf.Max(0, targetPosition.y + halfViewport.y - _maxBounds.RuntimeValue.y);
        var bottomOverlap = Mathf.Max(0, _minBounds.RuntimeValue.y - (targetPosition.y - halfViewport.y));
        var verticalMove = bottomOverlap - topOverlap;

        // - - Adjust horizontal position
        var rightOverlap = Mathf.Max(0, targetPosition.x + halfViewport.x - _maxBounds.RuntimeValue.x);
        var leftOverlap = Mathf.Max(0, _minBounds.RuntimeValue.x - (targetPosition.x - halfViewport.x));
        var horizontalMove = leftOverlap - rightOverlap;

        targetPosition += Vector3.up * verticalMove + Vector3.right * horizontalMove;



        // - Move to the target position, either instantly or smoothly
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
