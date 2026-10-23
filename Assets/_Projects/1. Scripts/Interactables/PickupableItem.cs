using System.Collections;
using UnityEngine;

public class PickupableItem : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("COLLISION")]
    [SerializeField] private LayerMask deviceLayer;
    [SerializeField] private CircleCollider2D coll;

    [Header("ITEM SETTINGS")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private string pickupCommand = "grab";

    public bool CanInteract => true;
    public string promptMessage => "Item";

    private GameObject currentInteractor;
    
    private void Start(){
        Debug.Assert(rb, "rb is empty");
        Debug.Assert(spriteRenderer, "spriteRenderer is empty");
        Debug.Assert(itemData, "itemData is empty");
        Debug.Assert(coll, "circleCollider is empty");

        EventBus.TypeCommandEmitted.AddListener(OnTypeCommandEmitted);
        SetItem(itemData);
    }

    void FixedUpdate(){
        if(BoxConfinement2D.Instance == null) return;
        if(!BoxConfinement2D.Instance.HasBounds) return;

        Vector2 pos = rb.position;
        Vector2 clamped = BoxConfinement2D.Instance.ClampBox(pos, coll);
        Vector2 delta = clamped - pos;

        if(delta.sqrMagnitude > 0.000001f){
            Vector2 normal = delta.normalized;
            float into = -Vector2.Dot(rb.linearVelocity, normal);

            if(into > 0f) rb.linearVelocity += normal * into;
            rb.position = clamped;
        }
    }

    public void OnHover(GameObject interactor) {currentInteractor = interactor;}
    public void OnUnhover(GameObject interactor) {currentInteractor = null;}
    public void OnInteract(GameObject interactor) {}

    public void SetItem(ItemData newItemData){
        itemData = newItemData;
        spriteRenderer.sprite = itemData.itemSprite;
        if(coll == null) coll = GetComponent<CircleCollider2D>();
    }

    public void SetVelocity(Vector2 direction, float speed){
        Vector2 finalDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.zero;
        rb.linearVelocity = finalDirection * speed;
    }

    private void OnTypeCommandEmitted(string command){
        if (pickupCommand != command || currentInteractor == null) return;

        if(!currentInteractor.TryGetComponent(out PlayerInteractor playerInteractor)) return;
        if(!currentInteractor.TryGetComponent(out PlayerInventory playerInventory)) return;

        if (!playerInventory.HasItem){
            playerInventory.GrabItem(itemData);
            playerInteractor.Refresh();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if((deviceLayer.value & (1 << collision.gameObject.layer)) > 0){
            Debug.Log("Touched Device");
        }   
    }

    void OnEnable() => SetConfinement(this, true);
    void OnDisable() => SetConfinement(this, false);
    IEnumerator SetConfinement(PickupableItem pi, bool value){
        yield return null;
        BoxConfinement2D.Instance.AdjustPickupableItems(pi, value);
    }
}
