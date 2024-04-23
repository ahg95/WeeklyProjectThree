using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    BoolVariable _playerIsInsideSafeZone;

    [SerializeField]
    FloatVariable _currentHealth;

    [SerializeField]
    SimpleGameEvent _healthDepleted;

    [SerializeField]
    BoolVariable _playerIsDodging;

    [SerializeField]
    RuntimeSet<Collider2D> _magicShapes;

    [Header("Parameters")]
    [SerializeField]
    float _damagePerSecond;

    float _maxHealth;

    private void Awake()
    {
        _maxHealth = _currentHealth.RuntimeValue;
    }

    void LateUpdate()
    {
        AdjustHealth();
    }

    void AdjustHealth()
    {
        // No need to reduce the health if it is already 0 or if the player is inside a safe zone
        if (_currentHealth.RuntimeValue == 0 || _playerIsInsideSafeZone.RuntimeValue || _playerIsDodging.RuntimeValue)
            return;


        // Check if the player is inside magic
        for (int i = 0; i < _magicShapes.Count; i++)
        {
            var magicShape = _magicShapes.Get(i);

            if (magicShape.gameObject.activeInHierarchy && magicShape.OverlapPoint((Vector2)transform.position))
                Die();
        }
    }

    public void ResetHealth()
    {
        _currentHealth.RuntimeValue = _maxHealth;
    }

    public void Die()
    {
        _currentHealth.RuntimeValue = 0;
        _healthDepleted.Raise();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            Die();
    }
}