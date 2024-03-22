using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SafeZone : MonoBehaviour
{
    static int _nrOfSafeZonesContainingPlayer;

    [SerializeField]
    BoolVariable _playerIsInsideSafeZone;

    static int _playerLayer;

    private void OnValidate()
    {
        _playerLayer = LayerMask.NameToLayer("Player");



        // Setup collider
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != _playerLayer)
            return;

        _nrOfSafeZonesContainingPlayer++;

        _playerIsInsideSafeZone.RuntimeValue = _nrOfSafeZonesContainingPlayer > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != _playerLayer)
            return;

        _nrOfSafeZonesContainingPlayer--;

        _playerIsInsideSafeZone.RuntimeValue = _nrOfSafeZonesContainingPlayer > 0;
    }
}
