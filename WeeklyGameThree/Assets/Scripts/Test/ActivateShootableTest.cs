using UnityEngine;

public class ActivateShootableTest : MonoBehaviour
{
    [SerializeField]
    Shootable _shootable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _shootable.OnHit();
        }
    }
}
