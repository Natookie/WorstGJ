using System.Collections.Generic;
using UnityEngine;

public class BoxConfinement2D : MonoBehaviour
{
    public static BoxConfinement2D Instance { get; private set; }

    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private float padding = 0f;

    public List<PickupableItem> pickupableItems = new();

    private Collider2D bounds;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    public void SetBounds(Collider2D newBounds) => bounds = newBounds;
    public bool HasBounds => bounds != null;

    public Vector2 ClampBox(Vector2 position){
        if(bounds == null || playerCollider == null) return position;
        return ClampPosition(position, playerCollider.bounds);
    }

    public Vector2 ClampBox(Vector2 position, Collider2D collider){
        if(bounds == null || collider == null) return position;
        return ClampPosition(position, collider.bounds);
    }

    private Vector2 ClampPosition(Vector2 position, Bounds colliderBounds){
        Bounds b = bounds.bounds;
        Vector2 half = colliderBounds.extents;

        float minX = b.min.x + padding + half.x;
        float maxX = b.max.x - padding - half.x;
        float minY = b.min.y + padding + half.y;
        float maxY = b.max.y - padding - half.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }

    public void AdjustPickupableItems(PickupableItem item, bool add){
        if(item == null) return;
        if(add){
            if(!pickupableItems.Contains(item)) pickupableItems.Add(item);
        }else{
            pickupableItems.Remove(item);
        }
    }
}