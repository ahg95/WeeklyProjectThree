using System;
using UnityEngine;

public class Shootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [SerializeField]
    GameObject _targetIndicator;

    [SerializeField]
    SpriteRenderer _renderer;

    public Action _WasShot;

    private void Awake()
    {
        _targetIndicator.SetActive(false);
    }

    public void OnHit()
    {
        _WasShot?.Invoke();
    }

    private void OnEnable()
    {
        _activeShootables.Add(this);
    }

    private void OnDisable()
    {
        _activeShootables.Remove(this);

        ShowAsNotTargetable();
    }

    public void ShowAsNotTargetable()
    {
        _renderer.color = new Color(1, 1, 1, 0.5f);
        _targetIndicator.SetActive(false);
    }

    public void ShowAsTargetable()
    {
        _renderer.color = Color.white;
        _targetIndicator.SetActive(false);
    }

    public void ShowAsTarget()
    {
        _renderer.color = Color.white;
        _targetIndicator.SetActive(true);
    }

    public void ResetRoomObject()
    {
        ShowAsNotTargetable();
    }
}
