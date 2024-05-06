using UnityEngine;

public class PlayerOutlineChanger : MonoBehaviour
{
    [SerializeField]
    Material _playerImageMaterial;

    [SerializeField]
    BoolVariable _playerIsDashing;

    [SerializeField]
    Color _dashOutlineColour;

    Color _outlineColour1;
    Color _outlineColour2;

    const string FIRSTOUTLINECOLORNAME = "_FirstOutlineColor";
    const string SECONDOUTLINECOLORNAME = "_SecondOutlineColor";

    private void OnEnable()
    {
        _outlineColour1 = _playerImageMaterial.GetColor(FIRSTOUTLINECOLORNAME);
        _outlineColour2 = _playerImageMaterial.GetColor(SECONDOUTLINECOLORNAME);
    }

    private void OnDisable()
    {
        _playerImageMaterial.SetColor(FIRSTOUTLINECOLORNAME, _outlineColour1);
        _playerImageMaterial.SetColor(SECONDOUTLINECOLORNAME, _outlineColour2);
    }

    private void LateUpdate()
    {
        if (_playerIsDashing.RuntimeValue) {
            _playerImageMaterial.SetColor(FIRSTOUTLINECOLORNAME, _dashOutlineColour);
            _playerImageMaterial.SetColor(SECONDOUTLINECOLORNAME, _dashOutlineColour);
        } else {
            _playerImageMaterial.SetColor(FIRSTOUTLINECOLORNAME, _outlineColour1);
            _playerImageMaterial.SetColor(SECONDOUTLINECOLORNAME, _outlineColour2);
        }
    }
}
