using UnityEngine;

public class Door : MonoBehaviour, RoomObject
{
    [Header("References")]
    [SerializeField]
    GameObject _obstacle;

    [Header("Game Variables")]
    [SerializeField]
    BoolVariable _allDetectorsInMagic;

    [SerializeField]
    BoolVariable _allMandatoryWaypointsReached;

    void LateUpdate()
    {
        _obstacle.SetActive(!_allDetectorsInMagic.RuntimeValue || !_allMandatoryWaypointsReached.RuntimeValue);
    }

    public void ResetRoomObject()
    {
        _obstacle.SetActive(!_allDetectorsInMagic.RuntimeValue || !_allMandatoryWaypointsReached.RuntimeValue);
    }
}
