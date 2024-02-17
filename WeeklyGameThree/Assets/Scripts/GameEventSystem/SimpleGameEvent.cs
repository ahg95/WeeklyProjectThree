using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newGameEvent", menuName = "GameEvent/Simple")]
public class SimpleGameEvent : ScriptableObject
{
    private readonly List<SimpleGameEventListener> listeners = new ();

    public void AddListener(SimpleGameEventListener simpleGameEventListener)
    {
        if (!listeners.Contains(simpleGameEventListener))
            listeners.Add(simpleGameEventListener);
    }

    public void RemoveListener(SimpleGameEventListener simpleGameEventListener)
    {
        listeners?.Remove(simpleGameEventListener);
    }

    public void Raise()
    {
        for (int i = listeners.Count - 1; 0 <= i; i--)
        {
            var listener = listeners[i];
            
            listener.OnEventRaised();
        }
    }
}