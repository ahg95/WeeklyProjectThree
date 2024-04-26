using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    PlayerMover _playerMover;

    [SerializeField]
    PlayerAimer _playerAimer;

    [SerializeField]
    PlayerProjectileShooter _playerProjectileShooter;

    [Header("Variables")]
    [SerializeField]
    Vector3Variable _spawnPosition;

    [SerializeField]
    BoolVariable _playerIsDead;

    PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new();

        _playerMover._ListenToInput = true;
        _playerAimer._ShowIndicator = false;
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    void Update()
    {
        if (_playerInput.Player.Shoot.WasPressedThisFrame() && !_playerIsDead.RuntimeValue)
        {
            _playerMover._ListenToInput = false;
            _playerAimer._ShowIndicator = true;
        }
    }

    private void LateUpdate()
    {
        // The PlayerShooter should be disabled in late update so that the PlayerShooter is not disabled before it shoots.
        if (_playerInput.Player.Shoot.WasReleasedThisFrame() && !_playerIsDead.RuntimeValue)
        {
            _playerMover._ListenToInput = true;
            _playerAimer._ShowIndicator = false;
        }
    }

    public void Die()
    {
        _playerMover._ListenToInput = false;
        _playerAimer._ShowIndicator = false;
    }

    public void Respawn()
    {
        transform.position = _spawnPosition.RuntimeValue;
        _rigidbody.velocity = Vector2.zero;

        _playerMover._ListenToInput = true;
        _playerAimer._ShowIndicator = false;

        _playerMover.ResetDashCooldown();
        _playerProjectileShooter.DeleteAllProjectiles();
    }
}
