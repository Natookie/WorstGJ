using UnityEngine;

public class BoxConfinement2D : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private float padding = 0f;

    private Collider2D bounds;

    public void SetBounds(Collider2D newBounds) => bounds = newBounds;

    public Vector2 ClampBox(Vector2 position){
        if(bounds == null) return position;

        Bounds b = bounds.bounds;
        Bounds p = playerCollider.bounds;

        Vector2 playerHalf = p.extents;

        float minX = b.min.x + padding + playerHalf.x;
        float maxX = b.max.x - padding - playerHalf.x;
        float minY = b.min.y + padding + playerHalf.y;
        float maxY = b.max.y - padding - playerHalf.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}