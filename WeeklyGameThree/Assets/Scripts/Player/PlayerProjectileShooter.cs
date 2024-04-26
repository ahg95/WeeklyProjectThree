using UnityEngine;

public class PlayerProjectileShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Projectile _projectilePrefab;

    [SerializeField]
    Transform _projectileContainer;

    [SerializeField]
    PlayerAimer _aimer;

    [Header("Variables")]
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [Header("Events")]
    [SerializeField]
    SimpleGameEvent _playerShot;

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 100)]
    float _range;

    float _squaredRange;

    [SerializeField]
    [Range(0, 45)]
    float _maximumAngleError;

    Shootable _target;

    PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _squaredRange = Mathf.Pow(_range, 2);
    }

    private void OnEnable()
    {
        _playerInput.Player.Shoot.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Shoot.Disable();
    }

    void LateUpdate()
    {
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
            var angle = Vector2.Angle(_aimer.AimDirection, delta);

            if (angle < Mathf.Min(targetDeltaAngle, _maximumAngleError))
            {
                // We have found a better target than before. Therefore save this as the new target
                _target = shootable;
                targetDeltaAngle = angle;
            }
        }

        if (_target != null)
            _target.ShowAsTarget();



        // Shoot if the player releases the button
        if (_playerInput.Player.Shoot.WasReleasedThisFrame() && _target != null)
        {
            _playerShot.Raise();

            var projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity, _projectileContainer);

            projectile.Setup(_aimer.AimDirection, _target);
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
}
