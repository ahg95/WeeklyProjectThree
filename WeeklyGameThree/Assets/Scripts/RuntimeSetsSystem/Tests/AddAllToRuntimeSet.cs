using UnityEngine;

public abstract class AddAllToRuntimeSet<T> : MonoBehaviour where T: Object
{
    [SerializeField]
    RuntimeSet<T> _runtimeSet;

    private void OnEnable()
    {
        var objects = FindObjectsByType(typeof(T), FindObjectsSortMode.None);

        foreach (var obj in objects)
            _runtimeSet.Add((T)obj);
    }

    private void OnDisable()
    {
        _runtimeSet.Clear();
    }
}