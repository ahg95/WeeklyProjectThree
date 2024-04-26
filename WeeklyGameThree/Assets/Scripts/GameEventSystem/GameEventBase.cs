using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventBase<T> : ScriptableObject
{
    private readonly List<GameEventListenerBase<T>> _listeners = new ();

    public void AddListener(GameEventListenerBase<T> listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public void RemoveListener(GameEventListenerBase<T> listener)
    {
        _listeners.Remove(listener);
    }

    public void Raise(T args)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            var listener = _listeners[i];
            
            listener.OnEventRaised(args);
        }
    }
}