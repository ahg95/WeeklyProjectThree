using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    [SerializeField]
    SimpleGameEvent _everythingMadeBlack;

    public void StartFade()
    {
        _animator.SetTrigger("Fade");
        Debug.Log("Fade!!!");
    }

    public void RaiseEventForEverythingMadeBlack()
    {
        _everythingMadeBlack.Raise();
    }
}
