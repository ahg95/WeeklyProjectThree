using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Projectile _projectile;

    [SerializeField]
    Transform _projectileContainer;

    [Header("Variables")]
    [SerializeField]
    ShootableRuntimeSet _shootables;

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 100)]
    float _range;

    float _squaredRange;

    [SerializeField]
    [Range(0, 45)]
    float _maximumAngleError;

    Shootable Target
    {
        get => _target;

        set
        {
            if (value != _target)
            {
                _target = value;
                _TargetChanged.Invoke(value);
            }
        }
    }

    Shootable _target;

    public Action<Shootable> _TargetChanged;

    [HideInInspector]
    public bool _ListenToInput;

    PlayerInput _playerInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnValidate()
    {
        _squaredRange = Mathf.Pow(_range, 2);
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Shoot.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Shoot.Disable();

        Target = null;
    }

    void Update()
    {
        if (!_ListenToInput)
        {
            Target = null;
            return;
        }


        Vector2 aimDirection;

        if (Mouse.current.rightButton.isPressed || Mouse.current.rightButton.wasReleasedThisFrame)
        {
            // Transform the mouse position to an input vector
            // - Check where the player has clicked on the ground
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = _camera.ScreenPointToRay(mousePosition);
            _groundPlane.Raycast(ray, out var enter);
            var clickedPosition = ray.GetPoint(enter);

            // - Map the clicked position to an input vector
            const float minimumDistance = 3;
            var delta = clickedPosition - transform.position;
            aimDirection = delta.magnitude >= minimumDistance ? delta.normalized : Vector2.zero;
        } else
        {
            aimDirection = _playerInput.Player.Move.ReadValue<Vector2>().normalized;
        }

        // Update the currently targeted shootable

        Shootable updatedTarget = null;

        if (aimDirection != Vector2.zero)
        {
            float targetAngle = Mathf.Infinity;

            for (int i = 0; i < _shootables.Count; i++)
            {
                var target = _shootables.Get(i);

                var delta = target.transform.position - transform.position;

                // Filter by distance
                var sqrDistance = Vector2.SqrMagnitude(delta);

                if (sqrDistance > _squaredRange)
                    continue;

                // Filter by angle
                var angle = Vector2.Angle(aimDirection, delta);

                if (angle > targetAngle || angle > _maximumAngleError)
                    continue;

                // We have found a better target than before. Therefore save this as the new target
                updatedTarget = target;
                targetAngle = angle;
            }
        }

        Target = updatedTarget;


        // Shoot if the player releases the button
        if (_playerInput.Player.Shoot.WasReleasedThisFrame() && Target != null)
        {
            var projectile = Instantiate(_projectile, transform.position, Quaternion.identity, _projectileContainer);

            projectile.Setup(aimDirection, Target);
        }
    }

    public void DeleteAllProjectiles()
    {
        var projectiles = _projectileContainer.GetComponentsInChildren<Projectile>();

        foreach (var projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (_playerInput == null)
            return;

        var _aimDirection = _playerInput.Player.Move.ReadValue<Vector2>().normalized;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_aimDirection);
    }
}
