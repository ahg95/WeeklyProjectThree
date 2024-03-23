using UnityEngine;

public interface RoomObject
{
    public void Enable() {
        ((MonoBehaviour)this).enabled = true;
    }

    public void Disable()
    {
        ((MonoBehaviour)this).enabled = true;
    }

    public void ResetRoomObject();
}
