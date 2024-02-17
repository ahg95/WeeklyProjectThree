using UnityEngine;
using UnityEngine.Events;

public abstract class GameEventListenerBase<T> : MonoBehaviour
{
    [SerializeField]
    private GameEventBase<T> _gameEvent;
    
    [SerializeField]
    private UnityEvent<T> _response;

    private void OnEnable()
    {
        _gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        _gameEvent.RemoveListener(this);
    }

    public void OnEventRaised(T args)
    {
        _response.Invoke(args);
    }
}
