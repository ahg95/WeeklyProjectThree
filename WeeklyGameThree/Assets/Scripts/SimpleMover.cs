using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class SimpleMover : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    Vector2 _targetVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        _targetVelocity = Input.GetAxisRaw("Vertical") * Vector2.up + Input.GetAxisRaw("Horizontal") * Vector2.right;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _targetVelocity;
    }
}
