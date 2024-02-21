using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    [SerializeField]
    Transform _playerModel;

    [SerializeField]
    Animator _animator;

    Vector2 _lastMovementDirection;

    public void SetPlayerMovementVector(Vector2 movement)
    {
        // Rotate player model
        var angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;

        if (movement == Vector2.zero)
            angle = Mathf.Atan2(_lastMovementDirection.x, _lastMovementDirection.y) * Mathf.Rad2Deg;
        else
            _lastMovementDirection = movement;

        _playerModel.rotation = Quaternion.AngleAxis(angle, Vector3.up);



        // Set movement speed
        _animator.SetFloat("MovementSpeed", movement.magnitude);
    }
}
