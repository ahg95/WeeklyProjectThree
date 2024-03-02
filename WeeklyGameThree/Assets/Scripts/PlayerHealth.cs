using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    FloatVariable _currentHealth;

    [SerializeField]
    SimpleGameEvent _healthDepleted;

    [SerializeField]
    RuntimeSet<Collider2D> _magicShapes;

    [Header("Parameters")]
    [SerializeField]
    float _damagePerSecond;

    List<Collider2D> _magicShapeColliders = new();

    float _maxHealth;


    private void Awake()
    {
        _maxHealth = _currentHealth.RuntimeValue;
    }

    void Update()
    {
        ReduceHealthIfInsideMagic();
    }

    void ReduceHealthIfInsideMagic()
    {
        // No need to reduce the health if it is already 0
        if (_currentHealth.RuntimeValue == 0)
            return;



        // Check how many magic shapes are over the player
        var checkPosition = transform.position;

        var numberOfOverlappingShapes = 0;

        for (int i = 0; i < _magicShapes.Count; i++)
        {
            var magicShape = _magicShapes.Get(i);
            if (magicShape.gameObject.activeInHierarchy && magicShape.OverlapPoint(checkPosition))
                numberOfOverlappingShapes++;

            numberOfOverlappingShapes += GetComponent<Collider>() ? 1 : 0;
        }

        foreach (var collider in _magicShapeColliders)
            numberOfOverlappingShapes += collider.OverlapPoint(checkPosition) ? 1 : 0;



        // If the number of magic shapes over the player is even then there is no magic
        if (numberOfOverlappingShapes % 2 == 0)
            return;



        // Apply the damage and raise an event when it is depleted
        _currentHealth.RuntimeValue = Mathf.Max(0, _currentHealth.RuntimeValue - _damagePerSecond * Time.deltaTime);

        if (_currentHealth.RuntimeValue == 0)
        {
            _healthDepleted.Raise();
        }
    }

    public void ResetHealth()
    {
        _currentHealth.RuntimeValue = _maxHealth;
    }
}
