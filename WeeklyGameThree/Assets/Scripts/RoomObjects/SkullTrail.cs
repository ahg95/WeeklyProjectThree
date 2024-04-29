using UnityEngine;

public class SkullTrail : MonoBehaviour
{
    [SerializeField]
    ParticleSystem _particleSystem;

    Vector3 _previousPosition;

    void LateUpdate()
    {
        // Update particle system
        var forceOverLifetime = _particleSystem.forceOverLifetime;
        var deltaNormalized = (_previousPosition - transform.position).normalized;
        forceOverLifetime.x = new ParticleSystem.MinMaxCurve(deltaNormalized.x * 3);
        forceOverLifetime.y = new ParticleSystem.MinMaxCurve(deltaNormalized.y * 3);

        _previousPosition = transform.position;
    }
}
