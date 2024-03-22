using System;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField]
    GameObject _targetMarker;

    static PlayerShooter _aimer;

    public Action _WasShot;

    private void Awake()
    {
        _targetMarker.SetActive(false);
    }

    public void OnHit()
    {
        _WasShot?.Invoke();
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
