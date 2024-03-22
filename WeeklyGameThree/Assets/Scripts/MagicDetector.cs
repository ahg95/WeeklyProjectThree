using UnityEngine;

public class MagicDetector : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Sprite _insideMagic;

    [SerializeField]
    Sprite _outsideMagic;

    [SerializeField]
    Collider2DRuntimeSet _activeMagicShapes;

    [SerializeField]
    BoolVariable _allDetectorsInMagic;

    static int _nrOfMagicDetectors = 0;

    static int _nrOfMagicDetectorsInMagic = 0;

    bool _isInMagic;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _nrOfMagicDetectors++;

        UpdateState();
    }

    private void OnDisable()
    {
        _nrOfMagicDetectors--;

        if (_isInMagic)
        {
            _isInMagic = false;
            _nrOfMagicDetectorsInMagic--;
        }
    }

    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        bool wasInMagic = _isInMagic;



        // Check if this object is inside magic
        _isInMagic = false;

        for (int i = 0; i < _activeMagicShapes.Count; i++)
        {
            var magicShape = _activeMagicShapes.Get(i);

            if (magicShape.OverlapPoint(transform.position))
                _isInMagic = !_isInMagic;
        }



        if (!wasInMagic && _isInMagic)
        {
            _nrOfMagicDetectorsInMagic++;
            _renderer.sprite = _insideMagic;
        }
        else if (wasInMagic && !_isInMagic)
        {
            _nrOfMagicDetectorsInMagic--;
            _renderer.sprite = _outsideMagic;
        }
            


        _allDetectorsInMagic.RuntimeValue = _nrOfMagicDetectorsInMagic == _nrOfMagicDetectors;
    }
}
