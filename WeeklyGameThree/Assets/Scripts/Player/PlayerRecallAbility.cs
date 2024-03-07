using System.Collections.Generic;
using UnityEngine;

public class PlayerRecallAbility : MonoBehaviour
{
    struct Timestamp
    {
        public Vector2 _Position;
        public Vector2 _Velocity;
    }

    [Header("References")]
    [SerializeField]
    Rigidbody2D _toRecall;

    [SerializeField]
    Transform _marker;

    [SerializeField]
    BoolVariable _playerControlsEnabled;

    [Header("Parameters")]
    [SerializeField]
    float _cooldown;

    PlayerInput _playerInput;

    float _recallStartTime;

    Timestamp _recall;

    Queue<Timestamp> _buffer = new();

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _recall = new Timestamp() {_Position = _toRecall.position, _Velocity = _toRecall.velocity };
    }

    private void OnEnable()
    {
        _playerInput.Player.Recall.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Recall.Disable();
    }

    void Update()
    {
        // Show marker
        _marker.transform.position = _recall._Position;
        _marker.gameObject.SetActive(_recallStartTime + _cooldown < Time.time);


        // Handle recalling
        if (_playerControlsEnabled.RuntimeValue && _playerInput.Player.Recall.WasPressedThisFrame() && _recallStartTime + _cooldown < Time.time)
            Recall();
    }

    private void FixedUpdate()
    {
        _buffer.Enqueue(new Timestamp { _Position = _toRecall.position, _Velocity = _toRecall.velocity });

        if (_buffer.Count >= _cooldown / Time.fixedDeltaTime)
            _recall = _buffer.Dequeue();
    }

    public void OnPlayerRespawn()
    {
        _buffer.Clear();
        _recallStartTime = Time.time;
    }

    void Recall()
    {
        _recallStartTime = Time.time;

        _toRecall.position = _recall._Position;
        _toRecall.velocity = _recall._Velocity;
    }
}
