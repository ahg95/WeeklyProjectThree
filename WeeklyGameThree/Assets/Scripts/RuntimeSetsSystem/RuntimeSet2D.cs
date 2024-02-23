using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet2D<T> : ScriptableObject, ISerializationCallbackReceiver
{
    private Dictionary<Vector2Int, T> _container = new();

    public int Count
    {
        get
        {
            return _container.Count;
        }

        private set
        {
            
        }
    }
    
    public bool Get(Vector2Int indices, out T result)
    {
        return _container.TryGetValue(indices, out result);
    }

    public Dictionary<Vector2Int, T>.ValueCollection GetValues()
    {
        return _container.Values;
    }

    public void Add(Vector2Int indices, T toAdd)
    {
        _container.TryAdd(indices, toAdd);
    }

    public void RemoveAt(Vector2Int indices)
    {
        _container.Remove(indices);
    }

    public bool ContainsIndices(Vector2Int indices)
    {
        return _container.ContainsKey(indices);
    }
    
    public T this[Vector2Int indices]
    {
        get { return _container[indices]; }
        set { _container[indices] = value; }
    }
    
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        _container.Clear();
    }
}
