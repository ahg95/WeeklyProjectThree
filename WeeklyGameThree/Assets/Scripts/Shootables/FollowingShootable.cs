using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    [Header("Parameters")]
    [SerializeField]
    float _maxSpeed;

    [SerializeField]
    AnimationCurve _acceleration;

    [SerializeField]
    float _stunDuration;

    float _stunStartTime = -Mathf.Infinity;

    Shootable _shootable;

    static Transform _toFollow;

    void Awake()
    {
        _shootable = GetComponent<Shootable>();


        // Find reference to player rigidbody
        if (_toFollow == null)
        {
            _toFollow = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void OnEnable()
    {
        _shootable._WasShot += OnShot;
    }

    private void OnDisable()
    {
        _shootable._WasShot -= OnShot;
    }

    void OnShot()
    {
        _stunStartTime = Time.time;
    }

    void FixedUpdate()
    {
        Vector2 input;

        if (_stunStartTime + _stunDuration > Time.time)
            input = Vector2.zero;
        else
            input = ((Vector2)_toFollow.transform.position - _rigidbody.position).normalized;

        _rigidbody.velocity = PlayerMover.CalculateNextVelocity(input, _rigidbody.velocity, _maxSpeed, _acceleration);
    }
}
