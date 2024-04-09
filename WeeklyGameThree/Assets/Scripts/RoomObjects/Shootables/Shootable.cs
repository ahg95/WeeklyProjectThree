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

    public bool _CanBeTargeted;

    public Action _WasShotAt;

    public Action<Vector2> _WasHit;

    private void Awake()
    {
        _targetIndicator.SetActive(false);

        _CanBeTargeted = true;
    }

    public void OnHit(Vector2 hitDirection)
    {
        _WasHit?.Invoke(hitDirection);
    }

    public void OnShotFiredAt()
    {
        _WasShotAt?.Invoke();
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
