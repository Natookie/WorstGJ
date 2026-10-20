using UnityEngine;

public class PickupableItem : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("ITEM SETTINGS")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private string pickupCommand = "grab";

    public bool CanInteract => true;
    public string promptMessage => "Item";

    private GameObject currentInteractor;
    
    private void Start()
    {
        Debug.Assert(rb, "rb is empty");
        Debug.Assert(spriteRenderer, "spriteRenderer is empty");
        Debug.Assert(itemData, "itemData is empty");

        EventBus.TypeCommandEmitted.AddListener(OnTypeCommandEmitted);

        SetItem(itemData);
    }

    public void OnHover(GameObject interactor) {currentInteractor = interactor;}

    public void OnUnhover(GameObject interactor) {currentInteractor = null;}

    public void OnInteract(GameObject interactor) {}

    public void SetItem(ItemData newItemData)
    {
        itemData = newItemData;

        spriteRenderer.sprite = itemData.itemSprite;
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        Vector2 finalDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.zero;
        rb.linearVelocity = finalDirection * speed;
    }

    private void OnTypeCommandEmitted(string command)
    {
        if (pickupCommand != command || currentInteractor == null) return;

        if(!currentInteractor.TryGetComponent(out PlayerInteractor playerInteractor)) return;
        if(!currentInteractor.TryGetComponent(out PlayerInventory playerInventory)) return;

        if (!playerInventory.HasItem)
        {
            playerInventory.GrabItem(itemData);
            playerInteractor.Refresh();
            Destroy(gameObject);
        }
    }
}
