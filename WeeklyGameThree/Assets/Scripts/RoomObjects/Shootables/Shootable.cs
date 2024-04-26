using System;
using UnityEngine;

public class Shootable : MonoBehaviour, RoomObject
{
    [SerializeField]
    ShootableRuntimeSet _activeShootables;

    [SerializeField]
    Animator _targetAnimator;

    [SerializeField]
    Animator _hitAnimator;

    [HideInInspector]
    public bool _CanBeTargeted;

    public Action _WasShotAt;

    public Action<Vector2> _WasHit;

    private void Awake()
    {
        _targetAnimator.SetBool("IsTarget", false);

        _CanBeTargeted = true;
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

    public void OnHit(Vector2 hitDirection)
    {
        _WasHit?.Invoke(hitDirection);
        _hitAnimator.SetTrigger("Hit");
    }

    public void OnShotFiredAt()
    {
        _WasShotAt?.Invoke();
    }

    public void ShowAsNotTargetable()
    {
        _targetAnimator.SetBool("IsTarget", false);
        _targetAnimator.SetBool("IsTargetable", false);
    }

    public void ShowAsTargetable()
    {
        _targetAnimator.SetBool("IsTarget", false);
        _targetAnimator.SetBool("IsTargetable", true);
    }

    public void ShowAsTarget()
    {
        _targetAnimator.SetBool("IsTarget", true);
        _targetAnimator.SetBool("IsTargetable", false);
    }

    public void ResetRoomObject()
    {
        ShowAsNotTargetable();
    }
}
