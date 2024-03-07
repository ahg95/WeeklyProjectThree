using UnityEngine;

public class MagicToggleShootable : MonoBehaviour
{
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
        _shootable._WasShot += OnShot;
    }

    private void OnDisable()
    {
        _shootable._WasShot -= OnShot;
    }

    void OnShot()
    {
        _scaleIsIncreasing = !_scaleIsIncreasing;
    }

    private void Update()
    {
        const float CHANGEDURATION = 0.3333f;
        const float MAXSCALE = 5f;

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
}
