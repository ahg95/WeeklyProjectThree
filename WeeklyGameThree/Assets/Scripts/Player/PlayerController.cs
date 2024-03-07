using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    PlayerMover _playerMover;

    [SerializeField]
    PlayerShooter _playerShooter;

    [Header("Variables")]
    [SerializeField]
    Vector3Variable _spawnPosition;

    PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new();

        _playerMover._ListenToInput = true;
        _playerShooter._ListenToInput = false;
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
        if (_playerInput.Player.Shoot.WasPressedThisFrame())
        {
            _playerMover._ListenToInput = false;
            _playerShooter._ListenToInput = true;
        }
    }

    private void LateUpdate()
    {
        // The PlayerShooter should be disabled in late update so that the PlayerShooter is not disabled before it shoots.
        if (_playerInput.Player.Shoot.WasReleasedThisFrame())
        {
            _playerMover._ListenToInput = true;
            _playerShooter._ListenToInput = false;
        }
    }

    public void Die()
    {
        _playerMover._ListenToInput = false;
        _playerShooter._ListenToInput = false;
    }

    public void Respawn()
    {
        transform.position = _spawnPosition.RuntimeValue;
        _rigidbody.velocity = Vector2.zero;

        _playerMover._ListenToInput = true;
        _playerShooter._ListenToInput = false;

        _playerMover.ResetDashCooldown();
        _playerShooter.DeleteAllProjectiles();
    }
}
