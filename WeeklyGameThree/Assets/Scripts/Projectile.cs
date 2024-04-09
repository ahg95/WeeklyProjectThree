using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    [Range(0, 1)]
    float _curveFactor;

    [SerializeField]
    [Range(0.01f, 1)]
    float _flyingDuration;

    [SerializeField]
    AnimationCurve _accelerationCurve;

    float _initialTargetDistance;

    Shootable _target;

    CubicBezier _movementCurve;

    float _timeOfCreation;

    private void Awake()
    {
        _timeOfCreation = Time.time;
    }

    public void Setup(Vector2 StartDirection, Shootable Target)
    {
        _target = Target;

        // Setup movement curve
        var targetPosition = Target.transform.position;

        StartDirection = StartDirection.normalized;

        if (_movementCurve == null)
            _movementCurve = new();

        _initialTargetDistance = Vector2.Distance(transform.position, targetPosition);

        _movementCurve._Points[0] = transform.position;
        _movementCurve._Points[1] = transform.position + (Vector3)StartDirection * _curveFactor * _initialTargetDistance;
        _movementCurve._Points[2] = targetPosition + (_movementCurve._Points[1] - targetPosition).normalized * _curveFactor * _initialTargetDistance;
        _movementCurve._Points[3] = targetPosition;

        // Invoke event of shootable
        Target.OnShotFiredAt();

    }

    void Update()
    {
        // Save the position before this projectile has been moved. Used to calculate the direction from which the shootable has been hit.
        var positionBeforeMove = transform.position;



        // Update the movement curve because the target might have moved
        var targetPosition = _target.transform.position;
        _movementCurve._Points[2] = targetPosition + (_movementCurve._Points[1] - targetPosition).normalized * _curveFactor * _initialTargetDistance;
        _movementCurve._Points[3] = targetPosition;


        // Move this projectile along the curve
        var t = Mathf.Clamp01((Time.time - _timeOfCreation) / (_flyingDuration * _initialTargetDistance));

        t = _accelerationCurve.Evaluate(t);

        transform.position = _movementCurve.Evaluate(t);

        // Hit the target and destroy this projectile if it is at the target position
        if (t == 1)
        {
            var delta = transform.position - positionBeforeMove;
            _target.OnHit(delta);
            Destroy(gameObject);
        }
    }
}
