using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    [Range(1, 10)]
    float _movementSpeed;

    PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    void Start()
    {

    }

    void Update()
    {
        // If the player is clicking: calculate the movement direction.

        // Get movement vector
        var movement = _playerInput.Player.Move.ReadValue<Vector2>();

        _rigidbody.velocity = movement * _movementSpeed;
    }
}
