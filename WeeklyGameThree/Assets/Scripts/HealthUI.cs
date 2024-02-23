using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    FloatVariable _playerHealth;

    [SerializeField]
    Slider _playerHealthSlider;

    void LateUpdate()
    {
        _playerHealthSlider.value = _playerHealth.RuntimeValue;
    }
}
