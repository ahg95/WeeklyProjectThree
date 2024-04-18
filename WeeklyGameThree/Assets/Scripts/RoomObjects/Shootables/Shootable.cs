using System;
using UnityEditor.Animations;
using UnityEngine;

public class Shootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [SerializeField]
    Animator _animator;

    public bool _CanBeTargeted;

    public Action _WasShotAt;

    public Action<Vector2> _WasHit;

    private void Awake()
    {
        _animator.SetBool("IsTarget", false);

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
        _animator.SetBool("IsTarget", false);
        _animator.SetBool("IsTargetable", false);
    }

    public void ShowAsTargetable()
    {
        _animator.SetBool("IsTarget", false);
        _animator.SetBool("IsTargetable", true);
    }

    public void ShowAsTarget()
    {
        _animator.SetBool("IsTarget", true);
        _animator.SetBool("IsTargetable", false);
    }

    public void ResetRoomObject()
    {
        ShowAsNotTargetable();
    }
}
