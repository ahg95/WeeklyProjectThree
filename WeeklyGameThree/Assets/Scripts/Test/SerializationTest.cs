using UnityEngine;

public class SerializationTest : MonoBehaviour
{
    public bool _validationTrigger;

    [SerializeField, HideInInspector]
    bool _test;

    private void Awake()
    {
        Debug.Log($"Test is {_test}!");
    }

    private void OnValidate()
    {
        Debug.Log("Called OnValidate!");
        _test = true;
    }
}
