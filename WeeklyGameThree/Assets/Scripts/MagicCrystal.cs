using UnityEngine;

public class MagicCrystal : MonoBehaviour
{
    [SerializeField]
    AnimationCurve _animationCurve;

    [SerializeField]
    BoolVariable _playerIsDodging;

    [SerializeField]
    GameObject _magic;

    float _timeOfActivation = -1000;

    void Update()
    {
        const float SIZE = 5f;
        const float ACTIVATIONDURATION = 3f;

        var timeSinceStart = Time.time - _timeOfActivation;

        _magic.transform.localScale = Vector3.one * _animationCurve.Evaluate(timeSinceStart) * SIZE;

        _magic.SetActive(_timeOfActivation + ACTIVATIONDURATION >= Time.time);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_playerIsDodging.RuntimeValue && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            _timeOfActivation = Time.time;
    }
}
