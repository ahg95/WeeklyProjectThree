using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    // Used for reading movement input when using the mouse
    [SerializeField]
    Camera _camera;

    [Header("Variables")]
    [SerializeField]
    BoolVariable _playerIsDashing;

    [SerializeField]
    BoolVariable _playerCanDash;

    [Header("Walking parameters")]
    [SerializeField]
    float _walkMaxSpeed;

    [SerializeField]
    AnimationCurve _walkAcceleration;

    [Header("Dashing parameters")]
    [SerializeField]
    float _dashMaxSpeed;

    [SerializeField]
    AnimationCurve _dashAcceleration;

    [Header("Dashing to movement transition parameters")]
    [SerializeField]
    float _minDashDuration;

    [SerializeField]
    float _maxDashDuration;

    [SerializeField]
    float _transitionDuration;

    PlayerInput _playerInput;

    float _dashPressTime;

    float _dashReleaseTime;

    Vector2 _dashDirection;

    Vector2 _movementInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    [HideInInspector]
    public bool _ListenToInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        ResetDashCooldown();
    }

    public void ResetDashCooldown()
    {
        _dashPressTime = -1000;
        _dashReleaseTime = -1000;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _movementInput = Vector2.zero;
    }

    void Update()
    {
        if (!_ListenToInput)
        {
            _movementInput = Vector2.zero;
            return;
        }



        // Read movement input
        // - If the player is using the mouse, take the mouse position as movement input
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



        // Update the direction to dash into
        if (_movementInput != Vector2.zero)
            _dashDirection = _movementInput;



        // Check if the player started dashing
        if (_playerCanDash.RuntimeValue && _playerInput.Player.Dash.WasPressedThisFrame())
        {
            _dashPressTime = Time.time;
            _dashReleaseTime = _dashPressTime + _maxDashDuration;

            _rigidbody.velocity = _dashDirection.normalized * _dashMaxSpeed;

            _playerCanDash.RuntimeValue = false;
        // Check if the player stopped dashing
        } else if (_playerIsDashing.RuntimeValue && _playerInput.Player.Dash.WasReleasedThisFrame())
        {
            var earliestReleaseTime = _dashPressTime + _minDashDuration;
            var latestReleaseTime = _dashPressTime + _maxDashDuration;
            _dashReleaseTime = Mathf.Clamp(Mathf.Min(_dashReleaseTime, Time.time), earliestReleaseTime, latestReleaseTime);
        }

        _playerIsDashing.RuntimeValue = _dashPressTime <= Time.time && Time.time <= _dashReleaseTime + _transitionDuration + 0.25f;
    }

    private void FixedUpdate()
    {
        // Convert the movement input to a velocity
        // - Calculate the next velocity if the player was dashing and if the player was moving
        var movementVelocity = CalculateNextVelocity(_movementInput, _rigidbody.velocity, _walkMaxSpeed, _walkAcceleration);
        var dashVelocity = CalculateNextVelocity(_movementInput, _rigidbody.velocity, _dashMaxSpeed, _dashAcceleration);

        var transitionProgress = Mathf.InverseLerp(_dashReleaseTime, _dashReleaseTime + _transitionDuration, Time.time);

        _rigidbody.velocity = Vector2.Lerp(dashVelocity, movementVelocity, transitionProgress);
    }

    public static Vector2 CalculateNextVelocity(Vector2 input, Vector2 currentVelocity, float maxSpeed, AnimationCurve acceleration)
    {
        if (input == Vector2.zero && currentVelocity == Vector2.zero)
            return Vector2.zero;

        Vector2 nextVelocity = currentVelocity;

        input = input / Mathf.Max(1, input.magnitude);

        var targetVelocity = input * maxSpeed;

        // Move the next velocity along the direction of target velocity

        // - The current velocity in the target direction
        
        Vector2 mainDirection;

        if (input == Vector2.zero)
        {
            mainDirection = currentVelocity.normalized;


            // Decelerate

            var velInMainDir = currentVelocity.magnitude;

            // - Do not decelerate below the target velocity
            var maxVelChange = velInMainDir - targetVelocity.magnitude;

            var curveX = -velInMainDir / maxSpeed;
            var velocityChange = Mathf.Min(acceleration.Evaluate(curveX) * maxSpeed, maxVelChange);

            nextVelocity -= mainDirection * velocityChange;
        } else
        {
            mainDirection = targetVelocity.normalized;

            var velInMainDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(mainDirection, currentVelocity) * Mathf.Deg2Rad);

            if (targetVelocity.magnitude < velInMainDir)
            {
                // Decelerate

                // - Do not decelerate below the target velocity
                var maxVelChange = velInMainDir - targetVelocity.magnitude;

                var curveX = -velInMainDir / maxSpeed;
                var velocityChange = Mathf.Min(acceleration.Evaluate(curveX) * maxSpeed, maxVelChange);

                nextVelocity -= mainDirection * velocityChange;

            }
            else if (targetVelocity.magnitude > velInMainDir)
            {
                // Accelerate

                // - Do not accelerate above the target velocity
                var maxVelChange = targetVelocity.magnitude - velInMainDir;

                var curveX = velInMainDir / maxSpeed;
                var velocityChange = Mathf.Min(acceleration.Evaluate(curveX) * maxSpeed, maxVelChange);

                nextVelocity += mainDirection * velocityChange;
            }



            // Move the next velocity perpendicular to target velocity
            var perpendicularDirection = Vector2.Perpendicular(mainDirection);

            // - The current velocity in the perpendicular direction
            var velInPerpDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(perpendicularDirection, currentVelocity) * Mathf.Deg2Rad);

            // - Always decelerate in the perpendicular direction
            var curveXPerp = -Mathf.Abs(velInMainDir) / maxSpeed;

            var velocityChangePerp = Mathf.Min(acceleration.Evaluate(curveXPerp) * maxSpeed, Mathf.Abs(velInPerpDir));

            nextVelocity += perpendicularDirection.normalized * (velInPerpDir > 0 ? -velocityChangePerp : velocityChangePerp);
        }


        return nextVelocity;
    }
}
