using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    BoolVariable _playerIsDead;

    [SerializeField]
    SimpleGameEvent _playerDied;

    [SerializeField]
    BoolVariable _playerIsDodging;

    [SerializeField]
    RuntimeSet<Collider2D> _magicShapes;

    [SerializeField]
    RuntimeSet<Collider2D> _safeZones;

    void LateUpdate()
    {
        // Kill the player if they are within magic, but not within a safe zone
        // - No need to kill the player if they are already dead or if they are dashing
        if (_playerIsDead.RuntimeValue || _playerIsDodging.RuntimeValue)
            return;


        // Check if the player is inside a safe zone
        for (int i = 0; i < _safeZones.Count; i++)
        {
            var safeZone = _safeZones.Get(i);

            if (safeZone.gameObject.activeInHierarchy && safeZone.OverlapPoint((Vector2)transform.position))
                return;
        }


        // Check if the player is inside magic
        for (int i = 0; i < _magicShapes.Count; i++)
        {
            var magicShape = _magicShapes.Get(i);

            if (magicShape.gameObject.activeInHierarchy && magicShape.OverlapPoint((Vector2)transform.position))
            {
                _playerDied.Raise();
                return;
            }

        }
    }
}