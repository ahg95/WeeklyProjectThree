using UnityEngine;

public interface RoomObject
{
    public void Enable() {
        ((MonoBehaviour)this).enabled = true;
    }

    public void Disable()
    {
        ((MonoBehaviour)this).enabled = false;
    }

    public void ResetRoomObject();
}