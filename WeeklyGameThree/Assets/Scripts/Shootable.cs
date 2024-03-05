using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField]
    GameObject _targetMarker;

    static PlayerShooter _aimer;

    private void Awake()
    {
        _targetMarker.SetActive(false);
    }

    public virtual void OnHit()
    {
        Debug.Log("Has been hit!");
    }

    private void OnEnable()
    {
        if (_aimer == null)
            _aimer = FindObjectOfType<PlayerShooter>(true);

        if (_aimer != null)
            _aimer._TargetChanged += OnAimerTargetChanged;
    }

    private void OnDisable()
    {
        if (_aimer != null)
            _aimer._TargetChanged -= OnAimerTargetChanged;
    }

    void OnAimerTargetChanged(Shootable target)
    {
        _targetMarker.SetActive(target == this);
    }
}
