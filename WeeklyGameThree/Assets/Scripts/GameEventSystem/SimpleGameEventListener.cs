using UnityEngine;
using UnityEngine.Events;

public class SimpleGameEventListener : MonoBehaviour
{
    [SerializeField]
    private SimpleGameEvent _gameEvent;

    [SerializeField]
    private UnityEvent _respone;

    private void OnEnable() => _gameEvent.AddListener(this);

    private void OnDisable() => _gameEvent.RemoveListener(this);

    public void OnEventRaised() => _respone.Invoke();
}
