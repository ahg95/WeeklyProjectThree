using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
    [SerializeField]
    Transform _spawnPoint;

    [Header("Variables")]
    [SerializeField]
    Vector3Variable _spawnPosition;

    [SerializeField]
    Vector2Variable _cameraBoundsMin;

    [SerializeField]
    Vector2Variable _cameraBoundsMax;

    [SerializeField]
    Collider2DRuntimeSet _activeMagicShapes;

    BoxCollider2D _collider;

    List<Collider2D> _magicShapes;

    private void Awake()
    {
        // Adjust collider
        _collider = GetComponent<BoxCollider2D>();

        AdjustColliderSizeToTilemaps();



        // Find magic shapes
        // - Also find inactive magic shapes
        var colliders = GetComponentsInChildren<Collider2D>(true);

        var magicLayerIndex = LayerMask.NameToLayer("Magic");

        _magicShapes = new();

        foreach (var collider in colliders)
            if (collider.gameObject.layer == magicLayerIndex)
                _magicShapes.Add(collider);
    }

    void AdjustColliderSizeToTilemaps()
    {
        // Calculate the bounds of all tilemaps combined
        Bounds bounds = new();

        var tileMaps = GetComponentsInChildren<Tilemap>();

        foreach (var tilemap in tileMaps)
        {
            tilemap.CompressBounds();

            var minX = Mathf.Min(tilemap.localBounds.min.x, bounds.min.x);
            var minY = Mathf.Min(tilemap.localBounds.min.y, bounds.min.y);

            var maxX = Mathf.Max(tilemap.localBounds.max.x, bounds.max.x);
            var maxY = Mathf.Max(tilemap.localBounds.max.y, bounds.max.y);

            bounds.min = new Vector3(minX, minY, 0);
            bounds.max = new Vector3(maxX, maxY, 0);
        }



        // Update the collider
        _collider.offset = bounds.center;
        _collider.size = bounds.size;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        UpdateVariablesIfPlayerInRoom(collision);
    }


    // OnTriggerStay2D is not called the first frame of the game if the player is in the room.
    // If only OnTriggerStay2D was used then the variables would not be updated before the first update loop.
    // If the variables are not updated before the first update loop then the camera does not snap instantly.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateVariablesIfPlayerInRoom(collision);
    }

    void UpdateVariablesIfPlayerInRoom(Collider2D collision)
    {
        if (_collider.OverlapPoint(collision.transform.position) && collision.gameObject.tag == "Player")
        {
            // Update spawn position
            _spawnPosition.RuntimeValue = _spawnPoint.transform.position;

            // Update bounds
            var minBounds = new Vector2(_collider.bounds.min.x, _collider.bounds.min.y);
            var maxBounds = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);

            _cameraBoundsMin.RuntimeValue = minBounds;
            _cameraBoundsMax.RuntimeValue = maxBounds;

            // Update magic shapes
            _activeMagicShapes.Clear();

            foreach (var shape in _magicShapes)
                _activeMagicShapes.Add(shape);
        }
    }
}
