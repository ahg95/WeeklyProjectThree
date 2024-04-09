using UnityEngine;

public class ToggleShootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Sprite _enabledSprite;

    [SerializeField]
    Sprite _disabledSprite;

    [SerializeField]
    Transform _magic;

    [SerializeField]
    AnimationCurve _scaleCurve;

    Shootable _shootable;

    float _alpha;

    bool _scaleIsIncreasing;

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
        _scaleIsIncreasing = !_scaleIsIncreasing;
        _renderer.sprite = _scaleIsIncreasing ? _enabledSprite : _disabledSprite;
    }

    private void Update()
    {
        const float CHANGEDURATION = 0.3333f;
        const float MAXSCALE = 6f;

        if (_scaleIsIncreasing && _alpha < 1)
        {
            _alpha = Mathf.Clamp01(_alpha + Time.deltaTime / CHANGEDURATION);
        }
        else if (!_scaleIsIncreasing && _alpha > 0)
        {
            _alpha = Mathf.Clamp01(_alpha - Time.deltaTime / CHANGEDURATION);
        }

        var factor = _scaleIsIncreasing ? _scaleCurve.Evaluate(_alpha) : 1 - _scaleCurve.Evaluate(1 - _alpha);

        _magic.localScale = Vector3.one * factor * MAXSCALE;
    }

    public void ResetRoomObject()
    {
        _alpha = 0;
        _scaleIsIncreasing = false;
        _renderer.sprite = _disabledSprite;
    }
}
