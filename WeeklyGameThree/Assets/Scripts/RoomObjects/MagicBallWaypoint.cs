using UnityEngine;

public class MagicBallWaypoint : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    public void ShowAsActive()
    {
        _animator.SetTrigger("Activate");
    }

    public void ShowAsActiveImmediately()
    {
        _animator.SetTrigger("ActivateImmediately");
    }

    public void ShowAsInactive()
    {
        _animator.SetTrigger("Deactivate");
    }

    public void ShowAsInactiveImmediately()
    {
        _animator.SetTrigger("DeactivateImmediately");
    }
}
