using UnityEngine;

public class FollowingShootable : MonoBehaviour, RoomObject
{
    [Header("References")]
    [SerializeField]
    Rigidbody2D _rigidbody;

    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Animator _spriteAnimator;

    [Header("Parameters")]
    [SerializeField]
    float _maxSpeed;

    [SerializeField]
    AnimationCurve _acceleration;

    [SerializeField]
    float _stunDuration;

    [SerializeField]
    AnimationCurve _hitVelocity;

    [SerializeField]
    float _invincibilityDuration;

    [SerializeField]
    float _activationRange;

    Vector2 _hitDirection;

    float _squaredActivationRange;

    float _shotTime;

    Shootable _shootable;

    static Transform _toFollow;

    Vector3 _initialPosition;

    bool _isSleeping = true;

    void Awake()
    {
        _shootable = GetComponentInChildren<Shootable>();


        _shotTime = -1000;


        _squaredActivationRange = Mathf.Pow(_activationRange, 2);


        // Set initial position. Used for resetting
        _initialPosition = transform.position;


        // Initialize hit direction
        _hitDirection = Vector2.zero;


        // At the start the player follower should sleep
        _isSleeping = true;


        // Find reference to player rigidbody
        if (_toFollow == null)
            _toFollow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        _shootable._WasHit += OnHit;

        _shootable._WasShotAt += OnShotAt;
    }

    private void OnDisable()
    {
        _shootable._WasHit -= OnHit;

        _shootable._WasShotAt -= OnShotAt;

        _rigidbody.velocity = Vector2.zero;
    }

    void OnHit(Vector2 hitDirection)
    {
        _shotTime = Time.time;

        _hitDirection = hitDirection.normalized;

        _renderer.material.SetInteger("_Flash", 1);
        _spriteAnimator.SetBool("Bite", false);
    }

    void OnShotAt()
    {
        _shootable._CanBeTargeted = false;
    }

    private void Update()
    {
        // Check if this skull should wake up
        if (_isSleeping)
        {
            _isSleeping = Vector3.SqrMagnitude(transform.position - _toFollow.position) > _squaredActivationRange;

            if (_isSleeping)
                return;
            else
                _spriteAnimator.SetBool("Bite", true);
        }


        // Flip sprite depending on the relative position to the transform to follow
        _renderer.flipX = transform.position.x < _toFollow.transform.position.x;


        // Make this shootable targetable again if the invincibility time has run out
        var endOfInvincibilityTime = _shotTime + _invincibilityDuration;

        if (Time.time - Time.deltaTime < endOfInvincibilityTime && Time.time >= endOfInvincibilityTime)
        {
            _renderer.material.SetInteger("_Flash", 0);
            _spriteAnimator.SetBool("Bite", true);
            _shootable._CanBeTargeted = true;
        }
    }

    void FixedUpdate()
    {
        if (_shotTime + _stunDuration > Time.time || _isSleeping)
        {
            // Push this shootable away from the shooting direction

            var timeSinceShot = Time.time - _shotTime;

            _rigidbody.velocity = -_hitDirection * _hitVelocity.Evaluate(timeSinceShot);
        }
        else
        {
            // Follow the target
            var input = ((Vector2)_toFollow.transform.position - _rigidbody.position).normalized;
            _rigidbody.velocity = PlayerMover.CalculateNextVelocity(input, _rigidbody.velocity, _maxSpeed, _acceleration);
        }
    }

    public void ResetRoomObject()
    {
        transform.position = _initialPosition;

        _rigidbody.velocity = Vector2.zero;

        _isSleeping = true;

        _shotTime = -1000;

        _shootable._CanBeTargeted = true;

        _renderer.material.SetInteger("_Flash", 0);

        _spriteAnimator.SetBool("Bite", false);
    }
}
