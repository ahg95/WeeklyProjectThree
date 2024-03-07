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
    BoolVariable _playerIsDodging;

    [Header("Movement parameters")]
    [SerializeField]
    [Range(1, 1000)]
    float _maxSpeed;

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

    Vector2 _dashDirection;

    float _speedBeforeDash;

    PlayerInput _playerInput;

    Vector2 _movementInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    [HideInInspector]
    public bool _ListenToInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        ResetDashCooldown();
    }

    public void ResetDashCooldown() => _dashStartTime = -Mathf.Infinity;

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

        }
        else
        {
            _movementInput = _playerInput.Player.Move.ReadValue<Vector2>();
        }


        // Update the direction to dash into
        if (_movementInput != Vector2.zero)
            _dashDirection = _movementInput;



        // Dash
        if (_playerInput.Player.Dash.WasPressedThisFrame() && _dashStartTime + _dashCooldown < Time.time)
        {
            _dashStartTime = Time.time;
            _speedBeforeDash = _rigidbody.velocity.magnitude;
            _rigidbody.velocity = _dashDirection.normalized * _dashSpeed;
            _playerIsDodging.RuntimeValue = true;
        }
    }

    private void FixedUpdate()
    {
        // Do not convert the movement input to a velocity if the player is dashing
        if (_dashStartTime + _dashDuration > Time.fixedTime)
            return;
        // Revert the velocity if the dash just ended
        else if (_dashStartTime + _dashDuration + Time.fixedDeltaTime > Time.fixedTime)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _speedBeforeDash;
            _playerIsDodging.RuntimeValue = false;
        }


        // Convert the movement input to a velocity
        _rigidbody.velocity = CalculateNextVelocity(_movementInput, _rigidbody.velocity, _maxSpeed, _acceleration);
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
        
        Vector2 mainDirection = targetVelocity == Vector2.zero ? Vector2.up : targetVelocity.normalized;

        /*
         * 
         *         Vector2 mainDirection;

        if (input == Vector2.zero)
            mainDirection = currentVelocity.normalized;
        else
            mainDirection = targetVelocity.normalized;
         */

        var velInMainDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(mainDirection, currentVelocity) * Mathf.Deg2Rad);

        if (targetVelocity.magnitude < velInMainDir)
        {
            // Decelerate

            // - Do not decelerate below the target velocity
            var maxVelChange = velInMainDir - targetVelocity.magnitude;

            var curveX = -velInMainDir / maxSpeed;
            var velocityChange = Mathf.Min(acceleration.Evaluate(curveX) * maxSpeed, maxVelChange);

            nextVelocity -= mainDirection * velocityChange;

        } else if (targetVelocity.magnitude > velInMainDir)
        {
            // Accelerate

            // - Do not accelerate above the target velocity
            var maxVelChange = targetVelocity.magnitude - velInMainDir;

            var curveX = velInMainDir / maxSpeed;
            var velocityChange = Mathf.Min(acceleration.Evaluate(curveX) * maxSpeed, maxVelChange);

            nextVelocity += mainDirection * velocityChange;
        }



        // Move the next velocity perpendicular to target velocity
        var secondaryDir = targetVelocity == Vector2.zero ? Vector2.left : Vector2.Perpendicular(targetVelocity);

        // - The current velocity in the perpendicular direction
        var velInPerpDir = currentVelocity.magnitude * Mathf.Cos(Vector2.Angle(secondaryDir, currentVelocity) * Mathf.Deg2Rad);

        // - Always decelerate in the perpendicular direction
        var curveXPerp = -Mathf.Abs(velInMainDir) / maxSpeed;

        var velocityChangePerp = Mathf.Min(acceleration.Evaluate(curveXPerp) * maxSpeed, Mathf.Abs(velInPerpDir));

        nextVelocity += secondaryDir.normalized * (velInPerpDir > 0 ? -velocityChangePerp : velocityChangePerp);



        return nextVelocity;
    }
}
