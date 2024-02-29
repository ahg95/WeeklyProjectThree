using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    PlayerRecorder _recorder;

    [SerializeField]
    Camera _camera;

    [Header("Variables")]
    [SerializeField]
    Vector3Variable _spawnPosition;

    [SerializeField]
    BoolVariable _playerControlsEnabled;

    [Header("Movement parameters")]
    [SerializeField]
    [Range(1, 1000)]
    float _maxSpeed;

    [SerializeField]
    [Range(0, 1)]
    float _accelerationFactor;

    [SerializeField]
    AnimationCurve _acceleration;

    [Header("Dashing")]
    [SerializeField]
    float _dashDuration;

    [SerializeField]
    float _dashSpeed;

    [SerializeField]
    float _dashCooldown;

    float _dashStartTime;

    Vector2 _facingDirection;

    PlayerInput _playerInput;

    Vector2 _movementInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    Vector2 _velocityBeforeDash;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _dashStartTime = -Mathf.Infinity;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    void Update()
    {
        if (!_playerControlsEnabled.RuntimeValue)
        {
            _movementInput = Vector2.zero;
            return;
        }



        // Handle dashing
        if (_playerInput.Player.Dash.WasPressedThisFrame() && _dashStartTime + _dashCooldown < Time.time)
        {
            _dashStartTime = Time.time;
            _velocityBeforeDash = _rigidbody.velocity;
            _rigidbody.velocity = _facingDirection.normalized * _dashSpeed;
        }



        // Read movement input
        if (Mouse.current.leftButton.isPressed)
        {
            // Transform the mouse position to an input vector
            // - Check where the player has clicked on the ground
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = _camera.ScreenPointToRay(mousePosition);
            _groundPlane.Raycast(ray, out var enter);
            var clickedPosition = ray.GetPoint(enter);

            // - Map the clicked position to an input vector
            const float minDistance = 0f;
            const float maxDistance = 1f;
            var delta = clickedPosition - transform.position;
            _movementInput = delta.normalized * Mathf.InverseLerp(minDistance, maxDistance, delta.magnitude);

        } else
        {
            _movementInput = _playerInput.Player.Move.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        // Do not convert the movement input to a velocity if the player is dashing
        if (_dashStartTime + _dashDuration > Time.fixedTime)
            return;
        // Revert the velocity if the dash just ended
        else if (_dashStartTime + _dashDuration + Time.fixedDeltaTime > Time.fixedTime)
            _rigidbody.velocity = _velocityBeforeDash;


        // Convert the movement input to a velocity
        _rigidbody.velocity = CalculateNextVelocity(_movementInput, _rigidbody.velocity);

        if (_rigidbody.velocity != Vector2.zero)
            _facingDirection = _rigidbody.velocity;
    }

    Vector2 CalculateNextVelocity(Vector2 input, Vector2 currentVelocity)
    {
        if (input == Vector2.zero && currentVelocity == Vector2.zero)
            return Vector2.zero;

        Vector2 nextVelocity = currentVelocity;

        var targetVelocity = input * _maxSpeed;

        // Move the next velocity along the direction of target velocity

        // - The current velocity in the target direction
        var targetDirection = targetVelocity == Vector2.zero ? Vector2.up : targetVelocity.normalized;

        var velInTargetDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(targetDirection, currentVelocity) * Mathf.Deg2Rad);


        if (targetVelocity.magnitude < velInTargetDir)
        {
            // Decelerate

            // - Do not decelerate below the target velocity
            var maxVelChange = velInTargetDir - targetVelocity.magnitude;

            var curveX = -velInTargetDir / _maxSpeed;
            var velocityChange = Mathf.Min(_acceleration.Evaluate(curveX) * _accelerationFactor * _maxSpeed, maxVelChange);

            nextVelocity -= targetDirection * velocityChange;

        } else if (targetVelocity.magnitude > velInTargetDir)
        {
            // Accelerate

            // - Do not accelerate above the target velocity
            var maxVelChange = targetVelocity.magnitude - velInTargetDir;

            var curveX = velInTargetDir / _maxSpeed;
            var velocityChange = Mathf.Min(_acceleration.Evaluate(curveX) * _accelerationFactor * _maxSpeed, maxVelChange);

            nextVelocity += targetDirection * velocityChange;
        }



        // Move the next velocity perpendicular to target velocity
        var perpDirection = targetVelocity == Vector2.zero ? Vector2.left : Vector2.Perpendicular(targetVelocity);

        // - The current velocity in the perpendicular direction
        var velInPerpDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(perpDirection, currentVelocity) * Mathf.Deg2Rad);

        // - Always decelerate in the perpendicular direction
        var curveXPerp = -Mathf.Abs(velInTargetDir) / _maxSpeed;
        var velocityChangePerp = Mathf.Min(_acceleration.Evaluate(curveXPerp) * _accelerationFactor * _maxSpeed, velInPerpDir);

        nextVelocity -= perpDirection.normalized * velocityChangePerp;



        return nextVelocity;
    }

    public void Respawn()
    {
        transform.position = _spawnPosition.RuntimeValue;
        _rigidbody.velocity = Vector2.zero;

        _dashStartTime = -Mathf.Infinity;
    }
}
