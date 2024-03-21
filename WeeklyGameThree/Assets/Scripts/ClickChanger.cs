using UnityEngine;

public class ClickChanger : MonoBehaviour
{
    // Movement
    Vector3 _initialPosition;

    [SerializeField]
    bool _move;

    [SerializeField]
    Transform _targetPosition;

    // Rotation
    [SerializeField]
    bool _rotate;

    [SerializeField]
    float _rotationAngle;

    // Scaling
    [SerializeField]
    bool _scale;

    float _initalScaleFactor;

    [SerializeField]
    float _targetScale;


    private void Awake()
    {
        _initialPosition = transform.position;
        _initalScaleFactor = transform.localScale.x;
    }


    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (Vector3.Distance(screenPosition, Input.mousePosition) >= 50)
            return;

        if (_move)
        {
            if (transform.position == _targetPosition.position)
                transform.position = _initialPosition;
            else if (transform.position == _initialPosition)
                transform.position = _targetPosition.position;
        }

        if (_rotate)
        {
            transform.RotateAround(transform.position, transform.forward, _rotationAngle);
        }

        if (_scale)
        {
            if (transform.localScale == Vector3.one * _targetScale)
                transform.localScale = Vector3.one * _initalScaleFactor;
            else
                transform.localScale = Vector3.one * _targetScale;
        }
    }
}
