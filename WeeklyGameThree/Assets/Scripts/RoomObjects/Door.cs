using UnityEngine;

public class Door : MonoBehaviour, RoomObject
{
    [Header("References")]
    [SerializeField]
    Collider2D _obstacle;

    [SerializeField]
    Animator _animator;

    [Header("Game Variables")]
    [SerializeField]
    BoolVariable _allDetectorsInMagic;

    [SerializeField]
    BoolVariable _allMandatoryWaypointsReached;

    void LateUpdate()
    {
        UpdateState();
    }

    public void ResetRoomObject()
    {
        UpdateState();
    }

    void UpdateState()
    {
        var closed = !_allDetectorsInMagic.RuntimeValue || !_allMandatoryWaypointsReached.RuntimeValue;

        _obstacle.enabled = closed;
        _animator.SetBool("Opened", !closed);
    }
}
