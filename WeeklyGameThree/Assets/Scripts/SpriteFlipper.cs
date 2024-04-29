using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _spriteRenderer;

    Vector3 _previousPosition;

    void LateUpdate()
    {
        var delta = (transform.position - _previousPosition);

        if (Mathf.Abs(delta.x) > Time.deltaTime * 0.1)
            _spriteRenderer.flipX = delta.x >= 0;

        _previousPosition = transform.position;
    }
}
