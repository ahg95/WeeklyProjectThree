using UnityEngine;

public class TimerChangeShootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    FloatVariable _timeScale;

    [SerializeField]
    FloatVariable _durationLeft;

    Shootable _shootable;

    static TimerChangeShootable _shotLast;

    const float TOTALDURATION = 2;
    const float TIMESCALE = 0.1f;

    private void Awake()
    {
        _shootable = GetComponent<Shootable>();
    }

    private void OnEnable()
    {
        _shootable._WasHit += OnHit;
    }

    private void OnDisable()
    {
        _shootable._WasHit -= OnHit;
    }

    void OnHit(Vector2 hitDirection)
    {
        _shotLast = this;
        _timeScale.RuntimeValue = TIMESCALE;
        _durationLeft.RuntimeValue = TOTALDURATION;
    }

    private void Update()
    {
        if (_shotLast != this)
            return;

        _durationLeft.RuntimeValue = Mathf.Max(_durationLeft.RuntimeValue -= Time.deltaTime, 0);

        if (_durationLeft.RuntimeValue > 0)
            return;

        _timeScale.RuntimeValue = 1;
    }

    public void ResetRoomObject()
    {
        _timeScale.RuntimeValue = 1;
        _durationLeft.RuntimeValue = 0;
        _shotLast = null;
    }
}
