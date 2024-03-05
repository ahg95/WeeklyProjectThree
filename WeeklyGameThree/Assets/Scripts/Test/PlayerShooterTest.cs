using UnityEngine;

public class PlayerShooterTest : MonoBehaviour
{
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [SerializeField]
    PlayerShooter _playerShooter;

    private void Awake()
    {
        var objects = FindObjectsOfType<Shootable>();

        foreach (var obj in objects)
        {
            _activeShootables.Add(obj);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _playerShooter.DeleteAllProjectiles();
    }
}
