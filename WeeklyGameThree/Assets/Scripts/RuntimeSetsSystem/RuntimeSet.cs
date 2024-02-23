using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject, ISerializationCallbackReceiver
{
    private List<T> _list = new();

    public int Count
    {
        get
        {
            return _list.Count;
        }

        private set
        {
            
        }
    }

    public T Get(int index)
    {
        return _list[index];
    }

    public bool Remove(T toRemove)
    {
        return _list.Remove(toRemove);
    }

    public void Add(T toAdd)
    {
        if (!_list.Contains(toAdd))
            _list.Add(toAdd);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        _list.Clear();
    }
}
