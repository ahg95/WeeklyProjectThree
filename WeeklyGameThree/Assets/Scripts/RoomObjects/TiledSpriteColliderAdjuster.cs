using UnityEngine;

public class TiledSpriteColliderAdjuster : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    BoxCollider2D _boxCollider;

    private void Awake()
    {
        _boxCollider.size = _spriteRenderer.size;
    }
}
