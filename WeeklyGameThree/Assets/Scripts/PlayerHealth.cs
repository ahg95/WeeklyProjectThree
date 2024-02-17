using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    FloatVariable _currentHealth;

    [SerializeField]
    float _damagePerSecond;

    [SerializeField]
    string _magicShapesTag;

    List<Collider2D> _magicShapeColliders = new();

    private void Awake()
    {
        FindMagicShapes();
    }

    public void FindMagicShapes()
    {
        _magicShapeColliders.Clear();

        var magicShapeObjects = GameObject.FindGameObjectsWithTag(_magicShapesTag);

        foreach (var magicShapeObject in magicShapeObjects)
        {
            var magicShapeCollider = magicShapeObject.GetComponent<Collider2D>();

            if (magicShapeCollider != null)
                _magicShapeColliders.Add(magicShapeCollider);
        }
    }

    void Update()
    {
        var checkPosition = transform.position;

        var numberOfOverlappingShapes = 0;

        foreach (var collider in _magicShapeColliders)
        {
            numberOfOverlappingShapes += collider.OverlapPoint(checkPosition) ? 1 : 0;
        }

        if (numberOfOverlappingShapes % 2 == 0)
            return;

        _currentHealth.RuntimeValue -= _damagePerSecond * Time.deltaTime;
    }
}
