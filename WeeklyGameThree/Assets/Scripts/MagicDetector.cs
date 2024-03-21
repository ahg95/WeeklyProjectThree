using Unity.VisualScripting;
using UnityEngine;

public class MagicDetector : MonoBehaviour
{
    static int _nrOfMagicDetectors = 0;

    static int _nrOfMagicDetectorsInMagic = 0;

    static bool _won;

    static Collider2D[] magics;

    bool _isInMagic;

    private void Awake()
    {
        if (magics == null)
            magics = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        _nrOfMagicDetectors++;

        UpdateState();
    }

    private void OnDisable()
    {
        _nrOfMagicDetectors--;

        UpdateState();
    }

    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        if (_won)
            return;



        bool wasInMagic = _isInMagic;

        foreach (var magic in magics)
        {
            if (magic.OverlapPoint(transform.position))
                _isInMagic = !_isInMagic;
        }

        if (!wasInMagic && _isInMagic)
        {
            _nrOfMagicDetectorsInMagic++;

            if (_nrOfMagicDetectorsInMagic == _nrOfMagicDetectors)
            {
                _won = true;
                Debug.Log("All in magic! You win!");
            }
        }
        else if (wasInMagic && !_isInMagic)
            _nrOfMagicDetectorsInMagic--;
    }
}
