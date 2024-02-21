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

    public BoxCollider2D _RoomCollider;

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

    private void Awake()
    {
        _cameraZ = _camera.transform.position.z;
    }

    void LateUpdate()
    {
        // Track the target
        // - Calculate the target position
        Vector3 targetPosition = _target.transform.position + Vector3.forward * _cameraZ + (Vector3)_targetOffset;



        // - Adjust the target position such that the camera viewport will stay within the room bounds,
        // or such that it centers the room staying within the bounds of the room is impossible
        var bounds = _RoomCollider.bounds;
        var halfViewport = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);

        // - - Adjust vertical position
        var topOverlap = Mathf.Max(0, targetPosition.y + halfViewport.y - bounds.max.y);
        var bottomOverlap = Mathf.Max(0, bounds.min.y - (targetPosition.y - halfViewport.y));
        var verticalMove = bottomOverlap - topOverlap;

        // - - Adjust horizontal position
        var rightOverlap = Mathf.Max(0, targetPosition.x + halfViewport.x - bounds.max.x);
        var leftOverlap = Mathf.Max(0, bounds.min.x - (targetPosition.x - halfViewport.x));
        var horizontalMove = leftOverlap - rightOverlap;

        targetPosition += Vector3.up * verticalMove + Vector3.right * horizontalMove;



        // - Smoothly move to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _smoothDampVelocity, _smoothTime, _maxSpeed, Time.deltaTime);
    }
}
