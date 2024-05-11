using UnityEngine;

public class BounceDeformationAnimator : MonoBehaviour
{
    [SerializeField]
    Transform _bouncer;

    [SerializeField]
    [Range(0.5f, 10)]
    float _frequency;

    [SerializeField]
    [Range(0.25f, 3)]
    float _duration;

    [SerializeField]
    [Range(0.05f, 1)]
    float _amplitude;

    [SerializeField]
    [Range(0, 0.5f)]
    float _verticalPhaseOffset;

    [SerializeField]
    [Range(0f, 10f)]
    float _collisionFactor;

    float _bounceStart = -100;

    float _impactFactor = 1;

    private void Update()
    {
        var bounceMagnitude = Mathf.Lerp(_amplitude * _impactFactor, 0, EaseOutQuad((Time.time - _bounceStart) / _duration));

        var scale = new Vector3();

        scale.x = 1 - bounceMagnitude * Mathf.Sin((Time.time - _bounceStart) * Mathf.PI * 2 * _frequency);
        scale.y = 1 + bounceMagnitude * Mathf.Sin(Mathf.Max(0, (Time.time - _bounceStart) * Mathf.PI * 2 * _frequency - Mathf.PI * _verticalPhaseOffset));
        scale.z = _bouncer.localScale.z;

        _bouncer.localScale = scale;
    }

    private float EaseOutQuad(float x)
    {
        x = Mathf.Clamp01(x);

        return 1 - (1 - x) * (1 - x);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var magnitude = collision.relativeVelocity.magnitude * _collisionFactor;

        _impactFactor = EaseOutQuad(magnitude);

        Bounce();
    }

    public void Bounce()
    {
        _bounceStart = Time.time;
    }
}
