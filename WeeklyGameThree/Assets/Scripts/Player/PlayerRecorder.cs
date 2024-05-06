using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _playerRigidbody;

    [SerializeField]
    Transform _playerModel;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    Texture _defaultTexture;

    [SerializeField]
    Texture _canDashTexture;

    [SerializeField]
    Material _material;

    [Header("Variables")]
    [SerializeField]
    BoolVariable _isDashing;

    [SerializeField]
    BoolVariable _canDash;

    [SerializeField]
    BoolVariable _isAiming;

    [SerializeField]
    Vector2Variable _aimingDirection;

    bool _isDying;

    private void LateUpdate()
    {
        // Switch out the material depending on if the player can dash or not
        if (_canDash.RuntimeValue)
            _material.mainTexture = _canDashTexture;
        else
            _material.mainTexture = _defaultTexture;

        _animator.SetBool("Dashing", _isDashing.RuntimeValue);

        _animator.SetBool("Aiming", _isAiming.RuntimeValue);

        var movement = _playerRigidbody.velocity;

        _animator.SetFloat("MovementSpeed", movement.magnitude);



        // Rotate player model
        if (!_isDying)
        {
            if (!_isAiming.RuntimeValue && movement != Vector2.zero) {
                var angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
                _playerModel.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            } else if (_isAiming.RuntimeValue) {
                var angle = Mathf.Atan2(_aimingDirection.RuntimeValue.x, _aimingDirection.RuntimeValue.y) * Mathf.Rad2Deg;
                _playerModel.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }
    }

    public void StartDying()
    {
        _animator.SetBool("Dead", true);
        _isDying = true;
    }

    public void StopDying()
    {
        _animator.SetBool("Dead", false);
        _isDying = false;
    }

    public void Shoot()
    {
        _animator.SetTrigger("Shoot");
    }
}
