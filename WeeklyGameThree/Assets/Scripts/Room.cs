using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
    static Room _activeRoom;

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

    [SerializeField]
    Collider2DRuntimeSet _activeSafeZones;

    [SerializeField]
    UnityEvent _roomEntered;

    [SerializeField, HideInInspector]
    BoxCollider2D _roomCollider;

    List<Shootable> _shootables = new();
    List<Collider2D> _magicShapes = new();
    List<Collider2D> _safeZones = new();

    List<RoomObject> _roomObjects = new();

    private void OnValidate()
    {
        _roomCollider = GetComponent<BoxCollider2D>();

        AdjustColliderSizeToTilemaps(_roomCollider);
    }

    void AdjustColliderSizeToTilemaps(BoxCollider2D boxCollider)
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
        boxCollider.offset = bounds.center;
        boxCollider.size = bounds.size;
    }

    private void Awake()
    {
        // Find magic shapes and safe zones
        var colliders = GetComponentsInChildren<Collider2D>(true);

        var magicLayerIndex = LayerMask.NameToLayer("Magic");
        var safeZoneLayerIndex = LayerMask.NameToLayer("SafeZone");

        foreach (var collider in colliders)
            if (collider.gameObject.layer == magicLayerIndex)
                _magicShapes.Add(collider);
            else if (collider.gameObject.layer == safeZoneLayerIndex)
                _safeZones.Add(collider);

        // Find shootables in the room
        _shootables = GetComponentsInChildren<Shootable>(true).ToList();

        // Find room objects in the room
        _roomObjects = GetComponentsInChildren<MonoBehaviour>(true).OfType<RoomObject>().ToList();

        // Disable this room
        enabled = false;
    }

    private void OnEnable()
    {
        // Disable the previous room, if there was one
        if (_activeRoom != null)
            _activeRoom.enabled = false;



        // Setup this room as the active one
        // - Save this room as the active one
        _activeRoom = this;

        // - Update the spawn position
        _spawnPosition.RuntimeValue = _spawnPoint.transform.position;

        // - Update the camera bounds
        var minBounds = new Vector2(_roomCollider.bounds.min.x, _roomCollider.bounds.min.y);
        var maxBounds = new Vector2(_roomCollider.bounds.max.x, _roomCollider.bounds.max.y);

        _cameraBoundsMin.RuntimeValue = minBounds;
        _cameraBoundsMax.RuntimeValue = maxBounds;

        // - Add all magic shapes in the room to the set of active magic shapes
        foreach (var shape in _magicShapes)
            _activeMagicShapes.Add(shape);

        // - Enable all room objects
        foreach (var roomObject in _roomObjects)
            roomObject.Enable();



        // Reset this room
        ResetRoomIfActive();



        // Raise event
        _roomEntered?.Invoke();
    }

    private void OnDisable()
    {
        // Disable all room objects
        foreach (var roomObject in _roomObjects)
            roomObject.Disable();



        // Remove all magic shapes in the room from the set of active magic shapes
        foreach (var magicShape in _magicShapes)
            _activeMagicShapes.Remove(magicShape);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_activeRoom == this || !_roomCollider.OverlapPoint(collision.transform.position) || collision.gameObject.tag != "Player")
            return;

        enabled = true;
    }

    // OnTriggerStay2D is not called the first frame of the game if the player is in the room.
    // If only OnTriggerStay2D was used then the variables would not be updated before the first update loop.
    // If the variables are not updated before the first update loop then the camera does not snap instantly.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_activeRoom == this || !_roomCollider.OverlapPoint(collision.transform.position) || collision.gameObject.tag != "Player")
            return;

        enabled = true;
    }

    public void ResetRoomIfActive()
    {
        if (_activeRoom != this)
            return;

        foreach (var roomObject in _roomObjects)
            roomObject.ResetRoomObject();
    }
}