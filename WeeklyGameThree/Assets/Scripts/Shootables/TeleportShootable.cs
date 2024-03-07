using UnityEngine;

public class TeleportShootable : MonoBehaviour
{
    Shootable _shootable;

    static Rigidbody2D _player;

    private void Awake()
    {
        _shootable = GetComponent<Shootable>();


        // Find reference to player rigidbody
        if (_player == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");

            _player = playerObject.GetComponent<Rigidbody2D>();
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
        _player.position = transform.position;
        gameObject.SetActive(false);
    }
}
