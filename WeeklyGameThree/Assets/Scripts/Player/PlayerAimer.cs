using System;
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
    Projectile _projectilePrefab;

    [SerializeField]
    Transform _projectileContainer;

    [SerializeField]
    LineRenderer _aimIndicator;

    [Header("Variables")]
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 100)]
    float _range;

    float _squaredRange;

    [SerializeField]
    [Range(0, 45)]
    float _maximumAngleError;

    [SerializeField]
    AnimationCurve _aimingAngularChange;

    Vector2 _shootingDirection = Vector2.right;

    Shootable _target;

    [HideInInspector]
    public bool _ListenToInput;

    PlayerInput _playerInput;

    Plane _groundPlane = new Plane(Vector3.back, Vector3.zero);

    private void Awake()
    {
        _playerInput = new PlayerInput();

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

        _target = null;
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
        if (!_ListenToInput)
        {
            // Disable the aim indicator
            _aimIndicator.enabled = false;
        } else
        {
            // Update the aim indicator
            _aimIndicator.enabled = true;
            _aimIndicator.positionCount = 2;
            _aimIndicator.SetPosition(0, transform.position);
            _aimIndicator.SetPosition(1, transform.position + (Vector3)_shootingDirection * _range);
        }



        // Update target and visuals of shootables
        _target = null;

        float targetDeltaAngle = Mathf.Infinity;

        for (int i = 0; i < _activeShootables.Count; i++)
        {
            var shootable = _activeShootables.Get(i);

            // Filter by targetable
            if (!shootable._CanBeTargeted)
            {
                shootable.ShowAsNotTargetable();
                continue;
            }

            // Hoist variable
            var delta = shootable.transform.position - transform.position;

            // Filter by distance
            var sqrDistance = Vector2.SqrMagnitude(delta);

            if (sqrDistance > _squaredRange)
            {
                shootable.ShowAsNotTargetable();
                continue;
            }

            shootable.ShowAsTargetable();

            // Filter by angle
            var angle = Vector2.Angle(_shootingDirection, delta);

            if (angle < Mathf.Min(targetDeltaAngle, _maximumAngleError))
            {
                // We have found a better target than before. Therefore save this as the new target
                _target = shootable;
                targetDeltaAngle = angle;
            }
        }



        // Shoot if the player releases the button
        if (_playerInput.Player.Shoot.WasReleasedThisFrame() && _target != null)
        {
            var projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity, _projectileContainer);

            projectile.Setup(_shootingDirection, _target);
        }



        UpdateActiveShootableVisuals();
    }

    void UpdateActiveShootableVisuals()
    {
        for (int i = 0; i < _activeShootables.Count; i++)
        {
            var shootable = _activeShootables.Get(i);

            var distance = Vector3.SqrMagnitude(shootable.transform.position - transform.position);

            if (distance > _squaredRange)
                shootable.ShowAsNotTargetable();
            else if (shootable == _target)
                shootable.ShowAsTarget();
            else
                shootable.ShowAsTargetable();
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
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_shootingDirection);
    }
}
