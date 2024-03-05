using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _playerRigidbody;

    [SerializeField]
    Transform _playerModel;

    [SerializeField]
    Animator _animator;

    bool _isDying;

    private void Update()
    {
        if (!_isDying)
        {
            ShowPlayerAnimationForMovement(_playerRigidbody.velocity);
        }
    }

    void ShowPlayerAnimationForMovement(Vector2 movement)
    {
        // Rotate player model
        if (movement != Vector2.zero)
        {
            var angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
            _playerModel.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        // Set movement speed
        _animator.SetFloat("MovementSpeed", movement.magnitude);
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
}
