using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DashCollectable : MonoBehaviour, RoomObject
{
    [SerializeField]
    BoolVariable _playerCanDash;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    ParticleSystem _defaultParticles;

    [SerializeField]
    ParticleSystem _burstParticles;

    bool _isCollected;

    static int _playerLayer = -1;

    private void Awake()
    {
        if (_playerLayer == -1)
            _playerLayer = LayerMask.NameToLayer("Player");

        _burstParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != _playerLayer || _playerCanDash.RuntimeValue || _isCollected)
            return;

        _playerCanDash.RuntimeValue = true;

        _defaultParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        _burstParticles.Stop();
        _burstParticles.Play();

        _animator.SetBool("Collected", true);

        _isCollected = true;
    }

    public void ResetRoomObject()
    {
        _playerCanDash.RuntimeValue = false;

        _defaultParticles.Play();

        _burstParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        _animator.SetBool("Collected", false);

        _isCollected = false;
    }
}
