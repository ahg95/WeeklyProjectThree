using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class KillOnTouch : MonoBehaviour
{
    [SerializeField]
    SimpleGameEvent _killedPlayer;

    [SerializeField]
    BoolVariable _playerIsDead;

    static int playerLayer = -1;

    private void Awake()
    {
        if (playerLayer == -1)
            playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerLayer || collision.gameObject.tag != "Hitbox" || _playerIsDead.RuntimeValue)
            return;

        _killedPlayer.Raise();
    }
}
