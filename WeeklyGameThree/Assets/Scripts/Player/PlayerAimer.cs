using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimer : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    LineRenderer _aimIndicator;

    [Header("Parameters")]
    [SerializeField]
    AnimationCurve _aimingAngularChange;

    [SerializeField]
    float _indicatorRange;

    Vector2 _shootingDirection = Vector2.right;

    [HideInInspector]
    public bool _ShowIndicator;

    PlayerInput _playerInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    public Vector2 AimDirection
    {
        get => _aimDirection;

        private set => _aimDirection = value;
    }

    Vector2 _aimDirection;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
    }

    void Update()
    {
        // Retrieve the input direction
        Vector2 input;

        if (Mouse.current.rightButton.isPressed || Mouse.current.rightButton.wasReleasedThisFrame)
        {
            // Transform the mouse position to an input vector
            // - Check where the player has clicked on the ground
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = _camera.ScreenPointToRay(mousePosition);
            _groundPlane.Raycast(ray, out var enter);
            var clickedPosition = ray.GetPoint(enter);

            // - Map the clicked position to an input vector
            input = (clickedPosition - transform.position).normalized;
        }
        else
        {
            input = _playerInput.Player.Move.ReadValue<Vector2>().normalized;
        }



        // Smoothly transition the shooting direction towards the input direction if the player makes an input
        if (input != Vector2.zero)
        {
            var angleToInput = Vector3.SignedAngle(_shootingDirection, input, Vector3.back);

            var deltaAngle = Mathf.Sign(angleToInput) * _aimingAngularChange.Evaluate(Mathf.Abs(angleToInput)) * Time.deltaTime;

            _shootingDirection = Quaternion.AngleAxis(deltaAngle, Vector3.back) * _shootingDirection;
        }



        // Update the shooting direction indicator
        if (!_ShowIndicator)
        {
            // Disable the aim indicator
            _aimIndicator.enabled = false;
        } else
        {
            // Update the aim indicator
            _aimIndicator.enabled = true;
            _aimIndicator.positionCount = 2;
            _aimIndicator.SetPosition(0, transform.position);
            _aimIndicator.SetPosition(1, transform.position + (Vector3)_shootingDirection * _indicatorRange);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_shootingDirection);
    }
}
