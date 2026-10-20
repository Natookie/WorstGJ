using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform interactFacing;

    [Header("SETTINGS")]
    [SerializeField] private PickupableItem pickupableItemPrefab;
    // [SerializeField] private string dropCommand = "drop";
    [SerializeField] private string throwCommand = "throw";
    [SerializeField] private float throwSpeed = 3.0f;

    public bool HasItem => holdingItemData != null;

    private ItemData holdingItemData;

    private void Start()
    {
        Debug.Assert(interactFacing, "interactFacing is missing");
        Debug.Assert(pickupableItemPrefab, "pickupableItemPrefab is empty");

        EventBus.TypeCommandEmitted.AddListener(OnTypeCommandEmitted);
    }

    public void GrabItem(ItemData newItemData)
    {
        holdingItemData = newItemData;

        // TODO: Erase if unused
        Debug.Log($"Grab {holdingItemData.itemName}");
    }

    // public void DropItem()
    // {
    //     if (!HasItem) return;

    //     // TODO: Erase if unused
    //     Debug.Log($"Drop {holdingItemData.itemName}");

    //     PickupableItem pickupableItemInstance = Instantiate(pickupableItemPrefab);
    //     pickupableItemInstance.transform.position = transform.position;

    //     // TODO: Set pickupableItemInstance parent if needed

    //     holdingItemData = null;
    // }

    public void ThrowItem()
    {
        if (!HasItem) return;

        // TODO: Erase if unused
        Debug.Log($"Throw {holdingItemData.itemName}");

        PickupableItem pickupableItemInstance = Instantiate(pickupableItemPrefab);
        pickupableItemInstance.transform.position = transform.position;

        pickupableItemInstance.SetItem(holdingItemData);
        pickupableItemInstance.SetVelocity(interactFacing.transform.up, throwSpeed);

        // TODO: Set pickupableItemInstance parent if needed

        holdingItemData = null;
    }

    private void OnTypeCommandEmitted(string command)
    {
        // if (command == dropCommand) DropItem();
        if (command == throwCommand) ThrowItem();
    }
}
